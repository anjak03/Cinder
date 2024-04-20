import numpy as np
import pandas as pd
from scipy.spatial import distance
from sklearn.preprocessing import StandardScaler
from scipy.spatial.distance import pdist, squareform


# TODO Recieving data from database @anastasija

# Sample data
data = {
    'FirstName': ['Anastasija', 'Bodan', 'Cjordan'],
    'LastName': ['Smith', 'Doe', 'Brown'],
    'Bio' : ["I love listening to rap music", "I playing ", "Basketball for life"], # TODO ChatGPT Bio similarity comparison @anja
    'Age': [25, 30, 35],
    'Faculty': ["CS", "EE", "Art"],
    'FacultyYear': [1, 3, 2],
    'Rating' : [1, 3, 10], # ne radi trenutno
    'Sex' : ['M', 'F','M'],
    'Employed': [True, False, True],
    'Employment': ["opis1", "opis2", "opis3"], # Ignorišemo trenutno 
    'Smoker': [False, False, True],
    'Pets': [True, False, True],
    'Languages': [['English', 'Spanish'], ['Dutch'], ['Spanish', 'German', 'English']],
    'Hobbies' : ["sport", "kafana", "muzika"]
}

df = pd.DataFrame(data)

# BOOLEAN ATRIBUTES HANDELING

# Convert boolean to int
df['Employed'] = df['Employed'].astype(int)
df['Smoker'] = df['Smoker'].astype(int)
df['Pets'] = df['Pets'].astype(int)

boolean_attributes = ['Employed', 'Smoker', 'Pets']
weights = np.array([1, 4, 3])  # Weights for Employed, Smoker, Pets respectively
weights = weights / weights.sum()

# Calculate weighted Hamming distance
def weighted_hamming_distance(x, y):
    return np.sum(weights * (x != y))

# Apply the custom distance function
boolean_distance_matrix = distance.pdist(df[boolean_attributes], metric=weighted_hamming_distance)
boolean_distance_df = pd.DataFrame(squareform(boolean_distance_matrix), index=df['FirstName'], columns=df['FirstName'])
user_boolean_sim = 1 / (1 + boolean_distance_df)

print(user_boolean_sim)

# NUMERICAL ATRIBUTES HANDELING

scaler = StandardScaler()
numerical_attributes = ['Age', 'FacultyYear']
df[numerical_attributes] = scaler.fit_transform(df[numerical_attributes])

# Calculate the Euclidean distance matrix for numerical attributes
numerical_distance_matrix = distance.cdist(df[numerical_attributes], df[numerical_attributes], 'euclidean')
numerical_distance_df = pd.DataFrame(numerical_distance_matrix, index=df['FirstName'], columns=df['FirstName'])
numerical_similarity_df = 1 / (1 + numerical_distance_df)


# Normalize/Standardize numeric columns
df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']] = scaler.fit_transform(df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']])

# Calculate the distance matrix
distance_matrix = distance.cdist(df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], 'euclidean')
distance_df = pd.DataFrame(distance_matrix, index=df['FirstName'], columns=df['FirstName'])
user_numerical_sim = 1 / (1 + distance_df)

# print(user_numerical_sim)

# LANGUAGE HANDELING 

# One-hot encoding of languages
languages = set(lang for sublist in df['Languages'] for lang in sublist)  # Unique languages
for language in languages:
    df[language] = df['Languages'].apply(lambda x: int(language in x))
df.drop('Languages', axis=1, inplace=True)


# Select only language columns for similarity calculation
language_columns = list(languages)  # Assuming 'languages' contains all language column names
jaccard_distances = pdist(df[language_columns], metric='jaccard')

# Convert the condensed distance matrix to a square matrix and then a DataFrame
user_language_dist_matrix = pd.DataFrame(squareform(jaccard_distances), index=df['FirstName'], columns=df['FirstName'])
user_language_sim = 1 - user_language_dist_matrix

# print(user_language_sim)

# FACULTY HANDELING

# Sample data of faculties and their categories
# TODO izvući ovo iz baze
faculty_data = {
    'CS': 'STEM',
    'EE': 'STEM',
    'Mechanical Engineering': 'STEM',
    'Biology': 'STEM',
    'Chemistry': 'STEM',
    'Psychology': 'Social Sciences',
    'Sociology': 'Social Sciences',
    'History': 'Humanities',
    'English': 'Humanities',
    'Philosophy': 'Humanities',
    'Art': 'Arts',
    'Music': 'Arts',
    'Theatre': 'Arts'
}

# Create DataFrame
faculties = pd.DataFrame(list(faculty_data.items()), columns=['Faculty', 'Category'])

def faculty_similarity(faculty1, faculty2, category_df):
    # Check if the same faculty
    if faculty1 == faculty2:
        return 1

    # Get categories for both faculties
    category1 = category_df.loc[category_df['Faculty'] == faculty1, 'Category'].values[0]
    category2 = category_df.loc[category_df['Faculty'] == faculty2, 'Category'].values[0]

    # Check if same category
    if category1 == category2:
        return 0.5

    # Different category
    return 0

# Initialize an empty DataFrame
faculty_similarity_matrix = pd.DataFrame(index=faculties['Faculty'], columns=faculties['Faculty'])

# Populate the matrix
for i in faculties['Faculty']:
    for j in faculties['Faculty']:
        faculty_similarity_matrix.at[i, j] = faculty_similarity(i, j, faculties)

# print(faculty_similarity_matrix)

user_faculty_sim = pd.DataFrame(0, index=df['FirstName'], columns=df['FirstName'])
for i in df['FirstName']:
    for j in df['FirstName']:
        fac1 = df[df['FirstName'] == i]['Faculty'].values[0]
        fac2 = df[df['FirstName'] == j]['Faculty'].values[0]
        user_faculty_sim.at[i, j] = faculty_similarity_matrix.at[fac1, fac2]

# print(user_faculty_sim)


### COMBINING MATRICES FOR END RESULT

weights = {'boolean' : 2, 'numerical' : 1, 'faculty': 1, 'language': 0.5 }
total_weight = sum(weights.values())
normalized_weights = {key: value / total_weight for key, value in weights.items()}

combined_similarity_df = (user_boolean_sim * normalized_weights['boolean'] +
                          user_numerical_sim * normalized_weights['numerical'] +
                          user_faculty_sim * normalized_weights['faculty'] +
                          user_language_sim * normalized_weights['language']
                          )

print(combined_similarity_df)
