import pandas as pd
from scipy.spatial import distance
from sklearn.preprocessing import StandardScaler

# Sample data
data = {
    'FirstName': ['Anastasija', 'Bodan', 'Cjordan'],
    'LastName': ['Smith', 'Doe', 'Brown'],
    'Bio' : ["Ja slušam samo gas muziku", "I am a cat person with 5 cats", "Basketball for life"],
    'Age': [25, 30, 35],
    'Faculty': ["FRI", "FE", "FS"],
    'FacultyYear': [1, 3, 2],
    'Rating' : [1, 3, 10], # ne radi trenutno
    'Sex' : ['M', 'F','M'],
    'Employed': [True, False, True],
    'Employment': ["opis1", "opis2", "opis3"], # Ignorišemo trenutno 
    'Smoker': [False, False, True],
    'Pets': [True, False, True],
    'Languages': [['English', 'Spanish'], ['English'], ['Spanish', 'German']],
    'Hobbies' : ["sport", "kafana", "muzika"]
}

df = pd.DataFrame(data)

# NUMERICAL AND BOOLEAN ATRIBUTES HANDELING

# Convert boolean to int
df['Employed'] = df['Employed'].astype(int)
df['Smoker'] = df['Smoker'].astype(int)
df['Pets'] = df['Pets'].astype(int)

# Normalize/Standardize numeric columns
scaler = StandardScaler()
df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']] = scaler.fit_transform(df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']])

# Calculate the distance matrix
distance_matrix = distance.cdist(df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], 'euclidean')

# Convert to a DataFrame
distance_df = pd.DataFrame(distance_matrix, index=df['FirstName'], columns=df['FirstName'])
euclidean_similarity_df = 1 / (1 + distance_df)

print(distance_df)

# END OF NUMERICAL AND BOOLEAN ATRIBUTES HANDELING

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
languages_sim_matrix = pd.DataFrame(squareform(jaccard_similarities), index=df['FirstName'], columns=df['FirstName'])

# print(languages_sim_matrix)

# END OF LANGUAGE HANDELING

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

def calculate_similarity(faculty1, faculty2, category_df):
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
        faculty_similarity_matrix.at[i, j] = calculate_similarity(i, j, faculties)

print(faculty_similarity_matrix)


# END OF FACULTY HANDELING 


### COMBINING MATRICES FOR END RESULT

# Combine matrices by averaging
combined_similarity_df = (languages_sim_matrix + euclidean_similarity_df) / 2

print(combined_similarity_df)
