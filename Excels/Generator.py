import os
import sys
import argparse
import pandas
import numpy


outputDir = None
currentConfigName = None
requireSuffix = "xlsx"
networkMode = False
dataBeginRowIndex = 2

class ConfigItem(object):
    def __init__(self) -> None:
        self.id = None
        self.datas = []
    
    def LoadMember(self, dataType, dataName, dataValue) -> None:
        result = self.ParseToStr(dataType, dataName, dataValue)
        if result is not None:
            self.datas.append(result)

    def ParseToStr(self, *args) -> list:
        results = []
        for arg in args:
            if arg is numpy.nan:
                return None
            results.append(str(arg))
        return results   

class ConfigClass(object):
    def __init__(self) -> None:
        self.id:int = 0
        self.members:list = []
        self.items:dict = {}


    def LoadXlsxData(self, xlsxDataFrame, variablesTypeSeries, variablesNameSeries) -> None:
        # print(xlsxDataFrame.shape[0], xlsxDataFrame.shape[1])

        for i in range(0, xlsxDataFrame.shape[1]):
            varType = variablesTypeSeries.iloc[i:i+1].tolist()[0]
            varName = variablesNameSeries.iloc[i:i+1].tolist()[0]
            member = (str(varType), str(varName))
            self.members.append(member)

        for i in range(dataBeginRowIndex, xlsxDataFrame.shape[0]):
            datasSeries = xlsxDataFrame.loc[i, :]

            id = datasSeries.iloc[0:1].tolist()[0]
            if id is numpy.nan:
                print(f"第{i+1}行数据项的id为空, 跳过...")
                continue

            cfg = ConfigItem()
            cfg.id = id

            for j in range(0, len(datasSeries)):
                item = (variablesTypeSeries.iloc[j:j+1].tolist()[0], 
                variablesNameSeries.iloc[j:j+1].tolist()[0], 
                datasSeries.iloc[j:j+1].tolist()[0])
                cfg.LoadMember(item[0], item[1], item[2])
                self.items[cfg.id] = cfg


    def GenerateCSharpFile(self, filePath:str) -> None:
        file = open(filePath, mode="w+", encoding="utf8")

        
        contentStr = """using System.Collections.Generic;
namespace XConfig 
{
public class %s
{
private %s(){ }
public class Data
{""" % (currentConfigName, currentConfigName)

        strMembers = ""

        for member in self.members:
            strMember = "public"
            for memberItem in member:
                strMember += " " + memberItem
            strMembers += "\n" + strMember + ";"
        contentStr += strMembers
        contentStr += "\n}\n"
        contentStr += "Dictionary<int, Data> _Items = new Dictionary<int, Data>()\n{"

        for key, item in self.items.items():
            strItemDic = "\n[%s] = new Data()\n{\n" % key
            for data in item.datas:
                strItemDic += "%s = " % data[1]
                if data[0] == "string":
                    strItemDic += "\"%s\",\n" % data[2]
                elif data[0] == "float":
                    strItemDic += "%sf,\n" % data[2]
                elif data[0].find("[]") > 0:
                    strItemDic += "new %s { %s },\n" % (data[1], data[2])
                else:
                    strItemDic += "%s,\n" % data[2]
            strItemDic += "},\n"
            contentStr += strItemDic

        contentStr += """};

static %s __item;
public static Dictionary<int, Data> Items{ get{ if (__item == null) __item = new %s();return __item._Items; }}

}
}""" % (currentConfigName, currentConfigName)
        file.write(contentStr)


if __name__ == "__main__":
    parse = argparse.ArgumentParser()
    parse.add_argument("--output", type=str, required=False, default='../Assets/Scripts/Game/GameConfigs', help='output direction')
    parse.add_argument("--networkmode",action="store_true",help="whether if the game is network game")

    args = parse.parse_args()
    outputDir = args.output
    networkMode = args.networkmode

    if networkMode:
        dataBeginRowIndex += 1

    for root, dirs, files in os.walk("."):
        for file in files:


            dotSplitList = file.split('.')
            if len(dotSplitList) < 2:
                print(f"file name: \n\"{file}\"\n is not correct.")
                continue
            shortName = dotSplitList[0]
            if "$" in shortName or "~" in shortName:
                continue
            suffix = dotSplitList[-1]
            if suffix != requireSuffix:
                continue

            fileName = os.path.join(root, file)

            excelDataFrameDic = pandas.read_excel(fileName, sheet_name=None)
            for sheetName, dataFrame in excelDataFrameDic.items():
                names = sheetName.split('|')
                if len(names) < 2:
                    print(f"{shortName}.{requireSuffix}: \nsheet name: \"{sheetName}\" is not correct")
                    input()
                    sys.exit(-1)
                commentName = names[0]
                currentConfigName = names[1]
                print(f"Generate {shortName}.{requireSuffix} -> {currentConfigName}.cs ...")
                variablesTypeSeries = dataFrame.iloc[0, :]
                variablesNameSeries = dataFrame.iloc[1, :]

                configClass = ConfigClass()
                configClass.LoadXlsxData(dataFrame, variablesTypeSeries, variablesNameSeries)
                configClass.GenerateCSharpFile(f"{outputDir}/{currentConfigName}.cs")
                print(f"Generate {commentName}: {currentConfigName}.cs compelete.")

    input("Enter any key to exit.")

