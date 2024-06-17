using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;


class Program
{
    private static EdgeDriver edgeDriver = null;
    static void Main(string[] args)
    {
        EdgeDriverService driverService = null;
        driverService = EdgeDriverService.CreateDefaultService();
        driverService.HideCommandPromptWindow = true;
        var options = new EdgeOptions();
        options.AddArgument("--headless");
        edgeDriver = new EdgeDriver(driverService, options, TimeSpan.FromMinutes(5));
        Console.WriteLine("Init Edge successfully!");
        try
        {
            edgeDriver.Navigate().GoToUrl("http://localhost:3000/users/sign_in");
            IWebElement usernameField = edgeDriver.FindElement(By.Id("email"));
            IWebElement passwordField = edgeDriver.FindElement(By.Id("password"));
            usernameField.SendKeys("user_name");
            passwordField.SendKeys("password");
            IWebElement loginButton = edgeDriver.FindElement(By.CssSelector("button[type='submit']"));
            loginButton.Click();
            IWebElement element = edgeDriver.FindElement(By.CssSelector("input[aria-label='search-1']"));
            string placeholder = element.GetAttribute("placeholder");
            Console.WriteLine("Placeholder: " + placeholder);
            Console.WriteLine();
            IReadOnlyCollection<IWebElement> tables = edgeDriver.FindElements(By.CssSelector(".table"));
            IWebElement table = tables.FirstOrDefault();
            if (table != null)
            {
                Console.WriteLine("Projects:");
                Console.WriteLine();
                var headers = table.FindElements(By.TagName("th"));
                var rows = table.FindElements(By.TagName("tr"));
                // Calculate maximum width needed for each column including headers and data cells
                List<string> headerTexts = headers.Select(header => header.Text).ToList();
                //Console.WriteLine("{0,-50} {1,-20} {2,-20} {3,-20} {4,-20}", headerTexts.ToArray());
                PrintRow(headerTexts.ToArray());
                Console.WriteLine();
                foreach (var row in rows)
                {
                    // Find all cells in the current row
                    var cells = row.FindElements(By.TagName("td"));
                    List<string> cellsText = cells.Select(cell => cell.Text).ToList();
                    if (cellsText.Count > 0) {
                        //Console.WriteLine("{0,-50} {1,-20} {2,-20} {3,-20} {4,-20}", cellsText.ToArray());
                        PrintRow(cellsText.ToArray());
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); // Waits for a key press from the user
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            edgeDriver.Quit();
            driverService.Dispose();
        }
    }

    static void PrintRow(params string[] columns)
    {
        // Define fixed widths for each column
        int[] columnWidths = { 20, 20, 20, 20, 20 };

        // Build the row string
        string row = "|";

        for (int i = 0; i < columns.Length; i++)
        {
            row += AlignCentre(columns[i], columnWidths[i]) + "|";
        }

        Console.WriteLine(row);
    }

    static string AlignCentre(string text, int width)
    {
        if (string.IsNullOrEmpty(text))
            return new string(' ', width);

        // Calculate total padding needed
        int totalPadding = width - GetDisplayWidth(text);
        int leftPadding = totalPadding / 2;
        int rightPadding = totalPadding - leftPadding;

        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
    }

    static int GetDisplayWidth(string text)
    {
        int width = 0;
        foreach (char c in text)
        {
            width += IsDoubleWidth(c) ? 2 : 1;
        }
        return width;
    }

    static bool IsDoubleWidth(char c)
    {
        // Example check for double-width characters (adjust as needed)
        return (c >= 0x1100 && c <= 0x115F) ||  // Hangul Jamo
               (c >= 0x2E80 && c <= 0xA4CF) || // CJK Radicals Supplement and others
               (c >= 0xAC00 && c <= 0xD7A3);   // Hangul Syllables
    }

}
