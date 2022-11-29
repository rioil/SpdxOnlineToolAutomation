using CsvHelper;
using SpdxOnlineToolAutomation;
using System.Globalization;
using System.Text.Json;

Console.WriteLine("SPDX Validator Automator");

// 検証対象リストのCSVファイルパス取得
var targetListCsv = args[0];

// 自動検証実行
using var automator = new SpdxValidatorAutomator(targetListCsv);
var result = await automator.ExecuteAsync();

// 検証結果ファイル保存
Directory.CreateDirectory("Results");
using var stream = File.OpenWrite($"Results/result_{DateTime.Now:yyyyMMdd-HHmmss}_{Path.GetFileName(targetListCsv)}.json");
using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = true });
JsonSerializer.Serialize(writer, result);
