from flask import Flask, request, jsonify
import re
import numpy as np
import pandas as pd
from scipy.spatial import distance
from sklearn.preprocessing import StandardScaler
from scipy.spatial.distance import pdist, squareform
import openai
from scipy.spatial.distance import cosine
import sys
import json


# OPENAI @Anja
BASE_URL = "https://openai-proxy.sellestial.com/api/"
API_KEY = "4ppeg2osd4nm"
client = openai.OpenAI(api_key=API_KEY, base_url=BASE_URL)

app = Flask(__name__)

@app.route('/recommend', methods=['POST'])
def recommend():
    users_data = request.json
    processed_data = process_user_data(users_data)
    return jsonify(processed_data)

def process_user_data(users_data):

    # Data format used for analyizing
    structured_data = {
        'id': [],
        'FirstName': [],
        'LastName': [],
        'Bio': [],
        'Age': [],
        'Faculty': [],
        'FacultyYear': [],
        'Rating': [],
        'Sex': [],
        'Employed': [],
        'Employment': [],
        'Smoker': [],
        'Pets': [],
        'Languages': [],
        'Hobbies': []
    }

    try:
        signed_user_id = users_data['existingUserId']
        user_list = users_data['users']
        for entry in user_list:
            structured_data['id'].append(entry['id'])
            structured_data['FirstName'].append(entry['firstName'])
            structured_data['LastName'].append(entry['lastName'])
            structured_data['Bio'].append(entry['bio'])
            structured_data['Age'].append(entry['age'])
            structured_data['Faculty'].append(entry['faculty']['name'] if entry['faculty'] else None)
            structured_data['FacultyYear'].append(entry['facultyYear'])
            structured_data['Rating'].append(entry['rating'])
            structured_data['Sex'].append(entry['sex'])
            structured_data['Employed'].append(entry['employed'])
            structured_data['Employment'].append(entry['employment'])
            structured_data['Smoker'].append(entry['smoker'])
            structured_data['Pets'].append(entry['pets'])
            structured_data['Languages'].append([lang['name'] for lang in entry['languages'] if lang['name'] != None])            
            structured_data['Hobbies'].append([hobby['name'] for hobby in (entry['hobbies'] if entry['hobbies'] else []) if hobby['name']])

    except Exception as e:
        import traceback
        traceback.print_exception(e)
        print("Database input error.") 

    df = pd.DataFrame(structured_data)
    df.set_index('id', inplace=True)

    # BOOLEAN ATRIBUTES HANDELING

    df['Employed'] = df['Employed'].astype(int)
    df['Smoker'] = df['Smoker'].astype(int)
    df['Pets'] = df['Pets'].astype(int)

    boolean_attributes = ['Employed', 'Smoker', 'Pets']
    weights = np.array([1, 4, 3])  # Weights for Employed, Smoker, Pets respectively
    weights = weights / weights.sum()

    def weighted_hamming_distance(x, y):
        return np.sum(weights * (x != y))

    # Apply the custom distance function
    boolean_distance_matrix = distance.pdist(df[boolean_attributes], metric=weighted_hamming_distance)
    boolean_distance_df = pd.DataFrame(squareform(boolean_distance_matrix), index=df.index, columns=df.index)
    user_boolean_sim = 1 / (1 + boolean_distance_df)


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


    # LANGUAGE HANDELING 

    # One-hot encoding of languages
    languages = set(lang for sublist in df['Languages'] for lang in sublist)  # Unique languages
    for language in languages:
        df[language] = df['Languages'].apply(lambda x: int(language in x))
    df.drop('Languages', axis=1, inplace=True)


    # Select only language columns for similarity calculation
    language_columns = list(languages)
    jaccard_distances = pdist(df[language_columns], metric='jaccard')

    # Convert the condensed distance matrix to a square matrix and then a DataFrame
    user_language_dist_matrix = pd.DataFrame(squareform(jaccard_distances), index=df.index, columns=df.index)
    user_language_sim = 1 - user_language_dist_matrix



    # FACULTY HANDELING

    # Faculty group mapping
    faculty_data = {
    "Academy of Music": "Arts",
    "Academy of Theatre, Radio, Film and Television": "Arts",
    "Academy of Fine Arts and Design": "Arts",
    "Biotechnical Faculty": "Natural Sciences",
    "School of Economics and Business": "Business",
    "Faculty of Architecture": "Engineering",
    "Faculty of Social Sciences": "Social Sciences",
    "Faculty of Electrical Engineering": "Engineering",
    "Faculty of Pharmacy": "Health Sciences",
    "Faculty of Civil and Geodetic Engineering": "Engineering",
    "Faculty of Chemistry and Chemical Technology": "Natural Sciences",
    "Faculty of Mathematics and Physics": "STEM",
    "Faculty of Maritime Studies and Transport": "Engineering",
    "Faculty of Computer and Information Science": "STEM",
    "Faculty of Social Work": "Social Sciences",
    "Faculty of Mechanical Engineering": "Engineering",
    "Faculty of Sport": "Health Sciences",
    "Faculty of Administration": "Business",
    "Faculty of Arts": "Arts",
    "Faculty of Medicine": "Health Sciences",
    "Faculty of Natural Sciences and Engineering": "Natural Sciences",
    "Faculty of Education": "Education",
    "Faculty of Law": "Law",
    "Faculty of Theology": "Humanities",
    "Veterinary Faculty": "Health Sciences",
    "Faculty of Health Sciences": "Health Sciences"
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

    user_faculty_sim = pd.DataFrame(0, index=df.index, columns=df.index)
    for i in df.index:
        for j in df.index:
            fac1 = df.at[i, 'Faculty']
            fac2 = df.at[j, 'Faculty']
            user_faculty_sim.at[i, j] = faculty_similarity_matrix.at[fac1, fac2]



    # HOBBY HANDELING


    # List of all hobbies options
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

    # BIO HANDELING

    def get_bio_similarity(bio1, bio2, model="gpt-4"):
        """Get bio similarity using OpenAI's chat completion."""
        try:
            response = client.chat.completions.create(
                model=model,
                messages=[
                    {"role": "system", "content": "Calculate the similarity between two bios and respond only with a float from 0-1."},
                    {"role": "user", "content": bio1},
                    {"role": "user", "content": bio2}
                ]
            )
            response_text = response.choices[0].message.content

            match = re.search(r'\b\d+\.\d+\b', response_text) # Find float in the response
            if match:
                return float(match.group(0))
            else:
                print("No float found in response.")
                return 0
        except Exception as e:
            print(f"Error in fetching similarity: {e}")
            return 0
        
    def calculate_bio_similarity(df):
        """Calculate bio similarity matrix using OpenAI completions."""
        n = len(df)
        bio_similarity = np.zeros((n, n))
        
        for i in range(n):
            for j in range(i + 1, n):
                sim = get_bio_similarity(df.iloc[i]['Bio'], df.iloc[j]['Bio'])
                bio_similarity[i][j] = bio_similarity[j][i] = sim

        return pd.DataFrame(bio_similarity, index=df.index, columns=df.index)

    user_bio_sim = calculate_bio_similarity(df)

    ### COMBINING MATRICES FOR END RESULT

    weights = { 'boolean'  : 2,     # smoking, pets, is working
                'numerical': 1,     # age, faculty year
                'faculty'  : 1,     # faculty group similarity
                'language' : 0.5,   # language jaccard
                'hobby'    : 2,     # hobby jaccard
                'bio'      : 2      # OpenAI AI text comparison
            }
    total_weight = sum(weights.values())
    normalized_weights = {key: value / total_weight for key, value in weights.items()}

    combined_similarity_df = (user_boolean_sim * normalized_weights['boolean'] +
                            user_numerical_sim * normalized_weights['numerical'] +
                            user_faculty_sim * normalized_weights['faculty'] +
                            user_language_sim * normalized_weights['language'] +
                            user_hobby_sim * normalized_weights['hobby'] +
                            user_bio_sim * normalized_weights['bio']
                            )


    def get_best_matches(user_id, combined_similarity_df):
        if user_id not in combined_similarity_df.index:
            return "User not found."

        user_similarities = combined_similarity_df.loc[user_id]

        # Drop the user's own entry to exclude them from their matches
        user_similarities = user_similarities.drop(user_id)
        best_matches = user_similarities.sort_values(ascending=False)

        # Convert the Series to a list of tuples (user_id, score)
        best_matches_list = list(best_matches.items())

        return best_matches_list

    output_data = get_best_matches(signed_user_id, combined_similarity_df)

    return {'status': 'success', 'data': output_data}


if __name__ == '__main__':
    app.run(debug=True, port=5000)
