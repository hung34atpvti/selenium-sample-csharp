using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


class Program
{
    private static ChromeDriver driver = null;
    static void Main(string[] args)
    {
        string filePath = Path.Combine(Environment.CurrentDirectory, "credentials.txt");


        var (email, password, url) = GetCredentials(filePath);
        Login(email, password, url);
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine(); // Waits for Enter key press (newline)
    }

    static void Login(string email, string password, string url)
    {
        ChromeDriverService driverService = null;
        driverService = ChromeDriverService.CreateDefaultService();
        driverService.HideCommandPromptWindow = true;
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        driver = new ChromeDriver(driverService, options, TimeSpan.FromMinutes(5));
        Console.WriteLine("Init Chrome successfully!");
        try
        {
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(600));
            var loginButton = wait.Until(drv => drv.FindElement(By.ClassName("ant-btn")));
            loginButton.Click();

            var emailField = wait.Until(drv => drv.FindElement(By.CssSelector("input[type='email']")));
            emailField.SendKeys(email);
            var nextButton = wait.Until(drv => drv.FindElement(By.CssSelector("input[type='submit']")));
            nextButton.Click();
            Thread.Sleep(1000);

            var passwordField = wait.Until(drv => drv.FindElement(By.CssSelector("input[type='password']")));
            passwordField.SendKeys(password);
            var signInButton = wait.Until(drv => drv.FindElement(By.CssSelector("input[type='submit']")));
            signInButton.Click();

            Thread.Sleep(1000);
            var OTP = wait.Until(drv => drv.FindElement(By.ClassName("displaySign")));
            if (OTP != null)
            {
                Console.Write("OTP: ");
                Console.Write(OTP.Text);
                Console.WriteLine();
            }
            var noButton = wait.Until(drv => drv.FindElement(By.Id("idBtn_Back")));
            noButton.Click();

            // Perform any additional steps if needed
            Console.WriteLine("Login successful!");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            driver.Quit();
            driverService.Dispose();
        }
    }

    static (string, string, string) GetCredentials(string filePath)
    {
        string email = null;
        string password = null;
        string url = null;

        try
        {
            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Split each line into key and value based on '='
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Assign email and password based on key
                    if (key.ToUpper() == "EMAIL")
                    {
                        email = value;
                    }
                    else if (key.ToUpper() == "PASSWORD")
                    {
                        password = value;
                    }
                    else if (key.ToUpper() == "URL")
                    {
                        url = value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading credentials file: {ex.Message}");
        }

        return (email, password, url);
    }
}
