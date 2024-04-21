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

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

```bash
dotnet new mvc
```

### Prerequisites

What things you need to install the software and how to install them:

```bash
python>=3.8
Flask
numpy
pandas
scipy
openai
```


### Run the application

python app.py
dotnet run


## Built With

- [Flask](https://flask.palletsprojects.com/en/2.0.x/) - The web framework used for building the API.
- [OpenAI](https://openai.com/api/) - AI service utilized for calculating bio similarity using GPT models.
- [NumPy](https://numpy.org/) - Fundamental package for scientific computing with Python.
- [Pandas](https://pandas.pydata.org/) - Library providing high-performance, easy-to-use data structures and data analysis tools.
- [SciPy](https://www.scipy.org/) - Library used for scientific and technical computing.

## Authors

- Anja Kuzevska - Backend and messaging system - [anjak03](https://github.com/anjak03)
- Aleksa Sibnović - User matching system  - [A-Sibi](https://github.com/A-Sibi)
- Jordan Lazov - Frontend and project design - [jaylzv](https://github.com/jaylzv)
- Anastasija Dzajkovska - Database and Module management - [anastasijadz](https://github.com/anastasijadz)