# Payments.Automation (Playwright + Reqnroll.xUnit + DI)

This targets .NET 8 with Playwright, Reqnroll.xUnit, xUnit.DependencyInjection while using Reqnroll's Built-in Formatter - HTML + Cucumber Messages (NDJSON). Add a "cucumber-json" formatter similarly (output e.g. report/cucumber.json

Getting started after downloading through whatever means, most likely via git clone. 

* Ensure you have Docket installed locally
* Then, use the following command from your terminal

1. `dotnet clean`
2. `dotnet nuget locals all --clear` (might not be needed when cloned freshly)
3. `dotnet build`
4. `docker compose up -d`
5. `dotnet test`

After the run, you should have:
1. tests/Tests.Automation/bin/Debug/net8.0/report/reqnroll_report.html
2. tests/Tests.Automation/bin/Debug/net8.0/report/messages.ndjson

**Note**: messages.ndjson can feed to downstream tools (CI dashboards, custom scripts, etc.). If a different HTML skin is needed, many Cucumber ecosystem tools accept this NDJSON directly.

Open the report using Chrome or any browser of choice; just replace it with Google Chrome. 
* `open -a "Google Chrome" tests/Tests.Automation/bin/Debug/net8.0/report/reqnroll_report.html`

Additionally, added the **Expressium.LivingDoc.ReqnrollPlugin** NuGet package to the ReqnRoll test project...
Setup the Expressium formatters properties in the configuration of the ReqnRoll test project...
Run the tests in the ReqnRoll test project and open the HTML report in the output directory:

* `open -a "Google Chrome" tests/Tests.Automation/bin/Debug/net8.0/LivingDoc.html`

To tear down Docker: `docker compose down`