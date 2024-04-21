# Cinder

# Strugling to find your perfect roommate? We got you covered!

This application is designed to match you with your best potential roommate. It uses a Hybrid matching system for precisely determining the similarity between people, making them an excelent choice for your future livning companion. It is safe to use, with two factor authentication and safe messaging system available for users that have approved each other. It consists of a clean look, simple yet effective User Experience and could potentialy provide service to vast amount of people.

This recommendation system is designed to provide personalized suggestions based on user profiles and preferences. It uses a combination of algorithms to calculate similarity scores across various attributes like bio, age, faculty, languages, and hobbies.

## Features

- Personalized user recommendations
- Robust user data processing
- Integration with OpenAI for advanced biography similarity assessment
- Custom real-time in-app messaging systems for connecting users
- Two factor authentication
- Mobile-first responsive web design

## API's used

- <b>OpenAI</b> for AI text description similaritiy scoring
- <b>REST API</b> for communication between python scripts and .NET main framework
- <b>SendGrid</b> for sending mail confirmations
- ...

## Getting Started


In order for the applicaton to work, first you need to run the Docker container.
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Cinder123*" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-CU8-ubuntu-20.04
```

After that, you need to create the Migrations folder and update the database:
```bash
dotnet ef migrations add INSERT_IME
dotnet ef database update
```

First python must run in the background. After that run the project using the dotnet run command.
```bash
python \RecommendationSystem\app.py
dotnet run
```

### Prerequisites

What things you need to install to run the software:

```bash
.NET 6.0 
Docker 
python>=3.8
Flask
numpy
pandas
scipy
openai
```

### Techonologies
Recommendation System:
- Python
    - numpy
    - scipy
    - pandas
    - Flask
    - openai
Backend:
- C#
- MySQL
- Docker
Frontend:
- Sass, automatically compiles to regular CSS.
- Razor pages, basic `.cshtml`, pages with scripts and included CSS stylesheet which were compiled from SCSS.


## Built With

- [Flask](https://flask.palletsprojects.com/en/2.0.x/) - The web framework used for building the API.
- [OpenAI](https://openai.com/api/) - AI service utilized for calculating bio similarity using GPT models.
- [NumPy](https://numpy.org/) - Fundamental package for scientific computing with Python.
- [Pandas](https://pandas.pydata.org/) - Library providing high-performance, easy-to-use data structures and data analysis tools.
- [SciPy](https://www.scipy.org/) - Library used for scientific and technical computing.
- [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr) - Used for real time messaging between matched users.
- [Docker](https://hub.docker.com/) - Container for the database.
- [.NetEf](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) - Database creation based on given models.
- [.NetIf](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio) - Login functionality for the users.
- [SendGrid](https://sendgrid.com/en-us) - Mail authentification and verification for users.

## Authors

- Anja Kuzevska - Backend and messaging system - [anjak03](https://github.com/anjak03)
- Aleksa Sibnović - User matching system  - [A-Sibi](https://github.com/A-Sibi)
- Jordan Lazov - Frontend and project design - [jaylzv](https://github.com/jaylzv)
- Anastasija Dzajkovska - Database and Module management - [anastasijadz](https://github.com/anastasijadz)