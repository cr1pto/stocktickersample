## Before you start:
- Docker must be installed on your machine prior to running the application. Please take note of the licensing requirements.
- You'll need to setup the project first. Run the `pre-install.ps1` powershell script and the `setup.ps1` powershell script.
- Update the `docker-compose.override.yaml` with the appropriate files.
- One of the scripts will generate a certificate and a password for the certificate. Please note the ceritificate name and update it here: `ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/{certificate file name here}` password and update the `docker-compose.override.yaml`: `ASPNETCORE_Kestrel__Certificates__Default__Password={certificate password}`.
- You'll need an API key from the financeapi.net site here: https://financeapi.net/.  Update the `docker-compose.override.yaml` with the appropriate api key: `StockTickerApiConfiguration__FinanceApiKey={finance api key here}`.

## Problems getting the application to run?
- Take a look at the scripts and run the script contents manually.  They are not idempotent sadly.  There is no error-handling either.  Please take a look at them before posting issues.

## The problem scope

This C# Project represents a solution to the following problem:

The Problem
John wants to be able to go to a website and enter any number of stock tickers on a single line, separated by spaces. Once he enters the tickers, he wants to press a button and see current prices and, if possible, other data such as price-to-earnings ratios and earnings-per-share in a grid format for all tickers entered.
The Solution
Please create a C# web application that can accomplish the problem above using the following technologies:
ASP.NET Core (.NET 5/6)
Razor Pages
C#
Bootstrap table/grid to present the results
For stock information use a free stock quoting API

