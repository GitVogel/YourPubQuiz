# ReadMe
Welkom to YourPubQuiz. The only place for all your pubquizing needs.

## Starting the application
- Make sure you have .NET 8.0 installed on your machine. You can download it from [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).


- Clone the repository to your local machine
- Navigate to ```...\Release```
- Right click in the folder and select "Open in Terminal". (Or open a terminal and navigate to the folder)
- Paste this command in the terminal: ```.\YourPubQuiz.exe --urls "http://localhost:5025"```
- You should see the message: ```Now listening on: http://localhost:5025``` in the terminal.
- Open your browser and navigate to http://localhost:5025
- Enjoy the quiz!

## Excecuting tests

### Backend
- Open the .sln file (in the root of the project).
- Go to the Test explorer of your IDE.
- You should see ```YourPubQuiz.Tests``` in the list of test projects.
- Run all the tests in that project. (Or run the tests you want to run)

### Frontend
- Make sure the application is running according to the instructions above.


- Navitgate to ```...\Frontend\YourPubQuiz```
- Right click in the folder and select "Open in Terminal". (Or open a terminal and navigate to the folder)
- Run the following commands:
  - ```npm install```
  - ```npx playwright install```
  - ```$env:BASE_URL='http://localhost:5025'; npx playwright test```
    - This ensures the correct port is used when running the tests. Otherwise the tests will fail.
