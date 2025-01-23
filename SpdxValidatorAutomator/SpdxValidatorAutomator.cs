using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using CsvHelper;
using System.Globalization;

namespace SpdxOnlineToolAutomation
{
    internal class SpdxValidatorAutomator : IDisposable
    {
        private readonly IWebDriver _driver;

        private readonly IWebElement _formatElement;
        private readonly IWebElement _fileElement;
        private readonly IWebElement _validateButton;
        private readonly IWebElement _modalDiv;
        private readonly IWebElement _modalTitle;
        private readonly IWebElement _modalBody;
        private readonly IWebElement _modalCloseButton;

        private readonly string _csvAbsolutePath;
        private readonly string _fileBasePath;

        public SpdxValidatorAutomator(string csvPath)
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl(@"https://tools.spdx.org/app/validate/");

            _formatElement = _driver.FindElement(By.Id("format"));
            //_fileElement = driver.FindElement(By.XPath("//input[@type='file']"));
            _fileElement = _driver.FindElement(By.Id("file"));
            _validateButton = _driver.FindElement(By.Id("validatebutton"));
            _modalDiv = _driver.FindElement(By.Id("myModal"));
            _modalTitle = _driver.FindElement(By.Id("modal-title"));
            _modalBody = _driver.FindElement(By.Id("modal-body"));
            _modalCloseButton = _driver.FindElement(By.XPath("//button[@data-dismiss=\"modal\"]"));

            _csvAbsolutePath = Path.IsPathFullyQualified(csvPath) ? csvPath : Path.GetFullPath(csvPath);
            _fileBasePath = Path.GetDirectoryName(_csvAbsolutePath)!;
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        public async Task<ValidationResult> ExecuteAsync()
        {
            var targetListFileReader = new CsvReader(new StreamReader(_csvAbsolutePath), CultureInfo.CurrentCulture);
            var validationTargets = targetListFileReader.GetRecordsAsync<ValidationTarget>();

            var started = DateTime.Now;
            var fileValidationResults = new List<FileValidationResult>();

            await foreach (var target in validationTargets)
            {
                fileValidationResults.Add(await ValidateFile(target));
            }

            return new ValidationResult(started, DateTime.Now, fileValidationResults);
        }

        private async Task<FileValidationResult> ValidateFile(ValidationTarget target)
        {
            _formatElement.SendKeys(target.FileFormat);
            _fileElement.SendKeys(GetAbsolutePath(target.FileName));
            _validateButton.Click();

            while (!_modalDiv.Displayed || !_modalCloseButton.Displayed)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
            var result = new FileValidationResult(target, _modalTitle.Text == "Success!", _modalTitle.Text, _modalBody.Text);

            _modalCloseButton.Click();

            return result;
        }

        private string GetAbsolutePath(string path)
        {
            return Path.IsPathFullyQualified(path) ? path : Path.GetFullPath(path, _fileBasePath);
        }
    }

    public record ValidationTarget(string FileFormat, string FileName);

    public record FileValidationResult(ValidationTarget Target, bool IsValid, string Title, string Description);

    public record ValidationResult(DateTime Started, DateTime Ended, List<FileValidationResult> FileValidationResults);
}
