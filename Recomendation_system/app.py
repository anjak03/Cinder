from flask import Flask, jsonify, request
import pandas as pd

app = Flask(__name__)

from sqlalchemy import create_engine
import pandas as pd

# Create a connection string - this example uses PostgreSQL, but you can adapt it for any database
database_url = "postgresql://username:password@localhost:5432/database_name"
engine = create_engine(database_url)

# Now you can use this engine to execute SQL queries or directly load SQL into a pandas DataFrame

# Write a SQL query to fetch user data along with joined language and hobby data
query = """
SELECT u.UserName, u.FirstName, u.LastName, u.Age, u.Sex, u.Smoker, u.Pets,
       array_agg(l.LanguageName) as Languages,
       array_agg(h.HobbyName) as Hobbies
FROM Users u
LEFT JOIN UserLanguages ul ON u.UserName = ul.UserName
LEFT JOIN Languages l ON ul.LanguageId = l.LanguageId
LEFT JOIN UserHobbies uh ON u.UserName = uh.UserName
LEFT JOIN Hobbies h ON uh.HobbyId = h.HobbyId
GROUP BY u.UserName
"""

# Load the data into a pandas DataFrame
data = pd.read_sql_query(query, engine)

# DATA EXTRACTED AT THIS POINT

#     FirstName LastName  Age     Sex  Smoker   Pets  IDFaculty                  Languages                       Hobbies
# 0       Alice Anderson   25  Female   False  False          3         [English, Spanish]            [Reading, Cooking]
# 1         Bob    Brown   22    Male    True   True          6                  [English]              [Hiking, Gaming]

data['Smoker'] = data['Smoker'].astype(int)
data['Pets'] = data['Pets'].astype(int)


@app.route('/match', methods=['POST'])
def match_roommate():
    # Extract the attributes of the logged-in user from the request
    user_data = request.json
    user_attributes = user_data['attributes']

    # Find the best match among potential roommates
    best_match = None
    best_score = float('-inf')  # Start with the worst possible score
    for roommate in potential_roommates:
        match_score = calculate_match_score(user_attributes, roommate['attributes'])
        if match_score > best_score:
            best_score = match_score
            best_match = roommate

    # If a match is found, return the roommate's details
    if best_match:
        return jsonify({'match': best_match}), 200
    else:
        return jsonify({'message': 'No match found'}), 404

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')
