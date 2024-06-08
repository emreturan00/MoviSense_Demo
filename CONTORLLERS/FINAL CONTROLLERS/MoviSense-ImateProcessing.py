import cv2
import mediapipe as mp
from scipy.spatial import distance as dist
import socket
import math

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_address = ('172.16.15.63', 12345)  # Change to Unity's address

# Default values for baseline (in cm)
baseline = 10

def calculate_depth_z(focal_length, baseline, distance):
    if distance != 0:
        return int((focal_length * baseline) / distance)
    return 0

def enhance_glove_visibility(frame):
    # Convert to grayscale
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    # Apply histogram equalization
    equalized = cv2.equalizeHist(gray)
    # Convert back to BGR for MediaPipe compatibility
    enhanced_frame = cv2.cvtColor(equalized, cv2.COLOR_GRAY2BGR)
    return enhanced_frame

def calculate_viewable_area(distance, horizontal_fov_deg=90, vertical_fov_deg=60):
    horizontal_fov_rad = math.radians(horizontal_fov_deg)
    vertical_fov_rad = math.radians(vertical_fov_deg)

    width = 2 * (distance * math.tan(horizontal_fov_rad / 2))
    height = 2 * (distance * math.tan(vertical_fov_rad / 2))

    return width, height

def calibrate_camera(mp_hands, hands, cap, known_distance_cm):
    print(f"Calibration: Place your hand at a known distance ({known_distance_cm} cm) from the camera.")

    while True:
        ret, frame = cap.read()
        if not ret:
            continue

        frame = cv2.flip(frame, 1)
        frame = enhance_glove_visibility(frame)
        frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = hands.process(frame_rgb)

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                wrist_landmark = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST]
                thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP]
                image_height, image_width, _ = frame.shape
                wrist_x = int(wrist_landmark.x * image_width)
                wrist_y = int(wrist_landmark.y * image_height)
                thumb_x = int(thumb_tip.x * image_width)
                thumb_y = int(thumb_tip.y * image_height)

                distance = dist.euclidean((wrist_x, wrist_y), (thumb_x, thumb_y))

                # Calculate the focal length
                focal_length = (distance * known_distance_cm) / baseline
                print(f"Calibration complete. Focal length: {focal_length:.2f} units.")
                return focal_length


def main():
    mp_hands = mp.solutions.hands
    hands = mp_hands.Hands(static_image_mode=False, max_num_hands=1, min_detection_confidence=0.7, min_tracking_confidence=0.5)

    cap = cv2.VideoCapture(0)
    # Set the resolution to 1920x1080 for Full HD
    # cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1920)
    # cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1080)

    try:
        # Ask the user for the known distance for calibration
        known_distance_cm = float(input("Enter the known distance for calibration (in cm): "))
        focal_length = calibrate_camera(mp_hands, hands, cap, known_distance_cm)

        # Calculate the viewable area at the given distance
        width, height = calculate_viewable_area(known_distance_cm)
        print(f"At {known_distance_cm} cm from the camera, the viewable area is:")
        print(f"Width: {width:.2f} cm")
        print(f"Height: {height:.2f} cm")

        while cap.isOpened():
            ret, frame = cap.read()
            if not ret:
                break

            frame = cv2.flip(frame, 1)
            frame = enhance_glove_visibility(frame)
            frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            results = hands.process(frame_rgb)

            if results.multi_hand_landmarks:
                for hand_landmarks in results.multi_hand_landmarks:
                    wrist_landmark = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST]
                    thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP]
                    image_height, image_width, _ = frame.shape
                    wrist_x = int(wrist_landmark.x * image_width)
                    wrist_y = int(wrist_landmark.y * image_height)
                    thumb_x = int(thumb_tip.x * image_width)
                    thumb_y = int(thumb_tip.y * image_height)

                    distance = dist.euclidean((wrist_x, wrist_y), (thumb_x, thumb_y))

                    data = f"{wrist_x},{wrist_y},{distance}"
                    sock.sendto(data.encode(), unity_address)
                    print("Data sent to Unity:", data)
                    

                    cv2.circle(frame, (wrist_x, wrist_y), 5, (0, 255, 0), -1)
                    cv2.line(frame, (wrist_x, wrist_y), (thumb_x, thumb_y), (255, 0, 0), 2)

            cv2.imshow('Frame', frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
    finally:
        cap.release()
        cv2.destroyAllWindows()

if __name__ == "__main__":
    main()
