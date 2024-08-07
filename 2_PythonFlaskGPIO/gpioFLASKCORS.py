from flask import Flask, jsonify
from flask_cors import CORS  # Import CORS for handling cross-origin requests
import RPi.GPIO as GPIO  # Import the GPIO library for interacting with the Raspberry Pi's GPIO pins

app = Flask(__name__)
CORS(app)  # Enable CORS for all routes to allow cross-origin requests

# GPIO setup
FHS = 17  # Replace with your actual GPIO pin number for the forward sensor
BHS = 27  # Replace with your actual GPIO pin number for the backward sensor

GPIO.setmode(GPIO.BCM)  # Set the GPIO mode to Broadcom SOC channel numbering
GPIO.setup(FHS, GPIO.IN, pull_up_down=GPIO.PUD_UP)  # Set the forward sensor pin as an input with pull-up resistor
GPIO.setup(BHS, GPIO.IN, pull_up_down=GPIO.PUD_UP)  # Set the backward sensor pin as an input with pull-up resistor

@app.route('/gpio', methods=['GET'])
def get_gpio_input():
    forward_value = GPIO.input(FHS)  # Read the input value from the forward sensor
    backward_value = GPIO.input(BHS)  # Read the input value from the backward sensor
    print(f"Forward: {forward_value}, Backward: {backward_value}")  # Print the sensor values for debugging
    return jsonify({'forward': forward_value, 'backward': backward_value})  # Return the sensor values as a JSON response

if __name__ == '__main__':
    try:
        app.run(host='0.0.0.0', port=5000, debug=True)  # Run the Flask app on all available interfaces, port 5000, with debug mode enabled
    except KeyboardInterrupt:
        print("\nConnection closed by user.")
    finally:
        GPIO.cleanup()  # Clean up the GPIO pins when the program is interrupted or exits
