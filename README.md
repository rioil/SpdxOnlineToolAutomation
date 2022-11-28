# SpdxOnlineToolAutomation

## SpdxValidatorAutomator

SPDX Online ToolのValidateの自動化を行います．

### 使用方法

`SpdxValidatorAutomator.exe` の第一引数に検証対象のファイルを記載したCSVファイルのパスを指定します．
検証対象を記載するCSVファイルは以下のように `<ファイルフォーマット>,<ファイル名>` の形式で記載します．

#### 利用可能なファイルフォーマット

|ファイルフォーマット|
|---|
|XLSX|
|XLS|
|RDFXML|
|TAG|
|JSON|
|YAML|
|XML|

#### CSVファイルの例

```CSV
FileFormat,FileName
JSON,"C:\Users\hoge\Documents\sample1.spdx.json"
TAG,"C:\Users\hoge\Documents\sample2.spdx"
```

### 実行結果

exe直下の`Results`フォルダ以下に，`result_<保存日時:yyyyMMdd-HHMMss></ｈyyyyMMdd-HHMMss>_<CSVファイルのファイル名>.json`を作成して出力します．
