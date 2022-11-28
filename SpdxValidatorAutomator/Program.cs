using CsvHelper;
using SpdxOnlineToolAutomation;
using System.Globalization;
using System.Text.Json;

Console.WriteLine("SPDX Validator Automator");

// 検証対象リストのCSVファイル読みこみ
var targetListCsv = args[0];
var targetListFileReader = new CsvReader(new StreamReader(targetListCsv), CultureInfo.CurrentCulture);
var targets = targetListFileReader.GetRecordsAsync<ValidationTarget>();

// 自動検証実行
using var automator = new SpdxValidatorAutomator();
var result = await automator.ExecuteAsync(targets);

// 検証結果ファイル保存
Directory.CreateDirectory("Results");
using var stream = File.OpenWrite($"Results/result_{DateTime.Now:yyyyMMdd-HHmmss}_{Path.GetFileName(targetListCsv)}.json");
using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = true });
JsonSerializer.Serialize(writer, result);
