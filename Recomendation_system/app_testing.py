import pandas as pd
from scipy.spatial import distance
from sklearn.preprocessing import StandardScaler

# Sample data
data = {
    'FirstName': ['Alice', 'Bob', 'Charlie'],
    'LastName': ['Smith', 'Doe', 'Brown'],
    'Age': [25, 30, 35],
    'Faculty': [1, 1, 2],
    'FacultyYear': [1, 3, 2],
    'Employed': [True, False, True],
    'Smoker': [False, False, True],
    'Languages': [['English', 'Spanish'], ['English'], ['Spanish', 'German']],
    'Pets': [True, False, True]
}

df = pd.DataFrame(data)

# LANGUAGE HANDELING 

# One-hot encoding of languages
languages = set(lang for sublist in df['Languages'] for lang in sublist)  # Unique languages
for language in languages:
    df[language] = df['Languages'].apply(lambda x: int(language in x))

# Drop the original Languages column if no longer needed
df.drop('Languages', axis=1, inplace=True)

from scipy.spatial.distance import pdist, squareform

# Select only language columns for similarity calculation
language_columns = list(languages)  # Assuming 'languages' contains all language column names
jaccard_similarities = pdist(df[language_columns], metric='jaccard')

# Convert the condensed distance matrix to a square matrix and then a DataFrame
languages_sim_matrix = pd.DataFrame(squareform(1 - jaccard_similarities), index=df['FirstName'], columns=df['FirstName'])

print("adfasdf")
print(languages_sim_matrix)

# END OF LANGUAGE HANDELING


# NUMERICAL AND BOOLEAN ATRIBUTES HANDELING


# Convert boolean to int
df['Employed'] = df['Employed'].astype(int)
df['Smoker'] = df['Smoker'].astype(int)
df['Pets'] = df['Pets'].astype(int)

# Normalize/Standardize numeric columns
scaler = StandardScaler()
df[['Age', 'Faculty', 'FacultyYear', 'Employed', 'Smoker', 'Pets']] = scaler.fit_transform(df[['Age', 'Faculty', 'FacultyYear', 'Employed', 'Smoker', 'Pets']])

# Calculate the distance matrix
distance_matrix = distance.cdist(df[['Age', 'Faculty', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], df[['Age', 'Faculty', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], 'euclidean')

# Convert to a DataFrame
distance_df = pd.DataFrame(distance_matrix, index=df['FirstName'], columns=df['FirstName'])
euclidean_similarity_df = 1 / (1 + distance_df)

print(distance_df)

# END OF NUMERICAL AND BOOLEAN ATRIBUTES HANDELING

### COMBINING MATRICES FOR END RESULT

# Combine matrices by averaging
combined_similarity_df = (languages_sim_matrix + euclidean_similarity_df) / 2

print(combined_similarity_df)
