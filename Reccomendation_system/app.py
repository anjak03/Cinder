from flask import Flask, jsonify, request

app = Flask(__name__)

# Mock data: list of potential roommates with their attributes
potential_roommates = [
    {'id': 1, 'name': 'Alice', 'attributes': {'cleanliness': 8, 'bedtime': '22:00', 'smoker': False}},
    {'id': 2, 'name': 'Bob', 'attributes': {'cleanliness': 6, 'bedtime': '23:00', 'smoker': True}},
    # Add more potential roommates
]

# Function to calculate match score based on user attributes
def calculate_match_score(user_attributes, roommate_attributes):
    score = 0

    # Example of a simple scoring system based on boolean values
    if user_attributes['Smoker'] == roommate_attributes['Smoker']:
        score += 10  # Arbitrary points for matching smoking preferences
    
    if user_attributes['Pets'] == roommate_attributes['Pets']:
        score += 10  # Points for matching pet preferences

    # Age comparison - you might allow some flexibility
    age_difference = abs(user_attributes['Age'] - roommate_attributes['Age'])
    if age_difference <= 5:
        score += 10 - age_difference  # Less points for greater age differences

    # Check if they are from the same faculty and year
    if user_attributes['Id_Faculty'] == roommate_attributes['Id_Faculty']:
        score += 20  # Higher points for being from the same faculty

    if user_attributes['FacultyYear'] == roommate_attributes['FacultyYear']:
        score += 10  # Points for being in the same academic year

    # Add more comparisons for Employment, Language, Hobbies, etc.
    # ...

    return score

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
