from flask import Flask, request, jsonify
import sys

app = Flask(__name__)

@app.route('/recommend', methods=['POST'])
def recommend():
    users_data = request.json
    processed_data = process_user_data(users_data)
    print(users_data)
    return jsonify(processed_data)

def process_user_data(users_data):
    # Your recommendation logic here
    # For now, we'll just return the received data for simplicity
    return {'status': 'success', 'data': users_data}


if __name__ == '__main__':
    app.run(debug=True, port=5000)
