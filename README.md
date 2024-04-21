# Cinder

# Recommendation System

This recommendation system is designed to provide personalized suggestions based on user profiles and preferences. It uses a combination of algorithms to calculate similarity scores across various attributes like bio, age, faculty, languages, and hobbies.

## Features

- Personalized user recommendations
- Integration with OpenAI for advanced bio similarity assessment
- Robust user data processing

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

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
### Installing

A step by step series of examples that tell you how to get a development environment running:

### Clone the repository
git clone https://github.com/anjak03/Cinder.git


# Install the requirements
pip install -r requirements.txt

# Run the application
python app.py


## Built With

- [Flask](https://flask.palletsprojects.com/en/2.0.x/) - The web framework used for building the API.
- [OpenAI](https://openai.com/api/) - AI service utilized for calculating bio similarity using GPT models.
- [NumPy](https://numpy.org/) - Fundamental package for scientific computing with Python.
- [Pandas](https://pandas.pydata.org/) - Library providing high-performance, easy-to-use data structures and data analysis tools.
- [SciPy](https://www.scipy.org/) - Library used for scientific and technical computing.
