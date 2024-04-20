import numpy as np
import pandas as pd
from scipy.spatial import distance
from sklearn.preprocessing import StandardScaler
from scipy.spatial.distance import pdist, squareform


# TODO Recieving data from database @anastasija

# Sample data
data = {
    'id' : [0, 1, 2],
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
    'Hobbies': [
        ["Reading", "Coding", "Traveling"],
        ["Gaming", "Hiking", "Traveling"],
        ["Cooking", "Traveling", "Reading"]
    ]
}

df = pd.DataFrame(data)
df.set_index('id', inplace=True)

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
boolean_distance_df = pd.DataFrame(squareform(boolean_distance_matrix), index=df.index, columns=df.index)
user_boolean_sim = 1 / (1 + boolean_distance_df)


# print(user_boolean_sim)

# NUMERICAL ATRIBUTES HANDELING

scaler = StandardScaler()
numerical_attributes = ['Age', 'FacultyYear']
df[numerical_attributes] = scaler.fit_transform(df[numerical_attributes])

# Calculate the Euclidean distance matrix for numerical attributes
numerical_distance_matrix = distance.cdist(df[numerical_attributes], df[numerical_attributes], 'euclidean')
numerical_distance_df = pd.DataFrame(numerical_distance_matrix, index=df.index, columns=df.index)
numerical_similarity_df = 1 / (1 + numerical_distance_df)



# Normalize/Standardize numeric columns
df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']] = scaler.fit_transform(df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']])

# Calculate the distance matrix
distance_matrix = distance.cdist(df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], df[['Age', 'FacultyYear', 'Employed', 'Smoker', 'Pets']], 'euclidean')
distance_df = pd.DataFrame(distance_matrix, index=df.index, columns=df.index)
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
user_language_dist_matrix = pd.DataFrame(squareform(jaccard_distances), index=df.index, columns=df.index)
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

user_faculty_sim = pd.DataFrame(0, index=df.index, columns=df.index)
for i in df.index:
    for j in df.index:
        fac1 = df.at[i, 'Faculty']
        fac2 = df.at[j, 'Faculty']
        user_faculty_sim.at[i, j] = faculty_similarity_matrix.at[fac1, fac2]


# print(user_faculty_sim)

# HOBBY HANDELING

# TODO Izvući ovo iz baze @anastasija

# List of all possible hobbies
all_hobbies = [
    "Reading", "Writing", "Coding", "Playing an instrument", "Drawing",
    "Cooking", "Gaming", "Hiking", "Photography", "Traveling", "Painting",
    "Gardening", "Playing sports", "Fishing", "Watching movies",
    "Listening to music", "Yoga", "Meditation", "Crafting", "Knitting",
    "Collecting", "Volunteering", "Learning a new language", "Birdwatching",
    "Astronomy", "Model building", "DIY Projects", "Geocaching", "Surfing",
    "Scuba diving"
]

for hobby in all_hobbies:
    df[hobby] = df['Hobbies'].apply(lambda x: int(hobby in x))
df.drop('Hobbies', axis=1, inplace=True)

# Calculate the Jaccard similarity
jaccard_distances = pdist(df[all_hobbies], metric='jaccard')
jaccard_similarity = 1 - squareform(jaccard_distances)  # Convert distance to similarity

# Convert the condensed similarity matrix to a DataFrame
user_hobby_sim = pd.DataFrame(jaccard_similarity, index=df.index, columns=df.index)


# print(user_hobby_sim)

### COMBINING MATRICES FOR END RESULT

weights = {'boolean'  : 2,    # smoking, pets, is working
           'numerical': 1,    # age, faculty year
           'faculty'  : 1,    # faculty similarity
           'language' : 0.5,  # language jaccard
           'hobby'    : 2     # hobby jaccard
           }
total_weight = sum(weights.values())
normalized_weights = {key: value / total_weight for key, value in weights.items()}

combined_similarity_df = (user_boolean_sim * normalized_weights['boolean'] +
                          user_numerical_sim * normalized_weights['numerical'] +
                          user_faculty_sim * normalized_weights['faculty'] +
                          user_language_sim * normalized_weights['language'] +
                          user_hobby_sim * normalized_weights['hobby'] 
                          )

# print(combined_similarity_df)

def get_best_matches(user_id, combined_similarity_df, num_matches=3):
    if user_id not in combined_similarity_df.index:
        return "User not found."

    # Retrieve the similarity scores for the given user
    user_similarities = combined_similarity_df.loc[user_id]

    # Drop the user's own entry to exclude them from their matches
    user_similarities = user_similarities.drop(user_id)

    # Sort the remaining users by similarity score in descending order
    best_matches = user_similarities.sort_values(ascending=False).head(num_matches)

    # Convert the Series to a list of tuples (user_id, score)
    best_matches_list = list(best_matches.items())

    return best_matches_list

def get_all_user_preferences(combined_similarity_df, num_matches=3):
    # Initialize an empty list to store preferences for each user
    all_preferences = []
    
    # Iterate over each user ID in the DataFrame index
    for user_id in combined_similarity_df.index:
        # Get the best matches for the current user
        best_matches = get_best_matches(user_id, combined_similarity_df, num_matches)
        
        # Append the list of best matches (list of tuples) to the overall list
        all_preferences.append(best_matches)
    
    return all_preferences

# Example usage:
all_user_preferences = get_all_user_preferences(combined_similarity_df)
for idx, preferences in enumerate(all_user_preferences):
    print(f"Preferences for User ID {idx}:")
    for match in preferences:
        print(f"    ID: {match[0]}, Score: {match[1]:.4f}")
    print()  # Adds a newline for better separation of output
