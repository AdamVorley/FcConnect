# FcConnect Application

## Description
This .NET Core application is designed to facilitate communication and updates for social workers and foster carers. Admin users (social workers) can create and assign surveys to regular users (foster carers) who can respond, as well as use the messaging functionality for communication. 

## Installation
1. Clone the repository to your local machine.
2. Ensure you have the .NET Core runtime installed. Note .NET 8 is required.
3. Navigate to the project directory and run `dotnet restore` to install dependencies.

## Configuration
If running the application locally, you will need to obtain a SendGrid API key for the application's email sending functionality. Follow these steps:
1. Sign up for a SendGrid account at [SendGrid](https://sendgrid.com/).
2. Generate an API key in your SendGrid account dashboard.
3. Follow the instructions provided in the Quickstart guide on the SendGrid site. This will involve setting your API key through the User Secret manager. Refer to .NET documentation here https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-8.0&tabs=visual-studio for further information.

##Usage
Run the application using dotnet run.
Access the application through your web browser at http://localhost:{port}.
Refer to the User Guide for information on the application's features and functionality.

## Deployed Application
The application is deployed on an Azure server at the following URL: https://fcconnect20240408121617.azurewebsites.net/. Test credentials to use the application can be found in the User Guide. 

		
