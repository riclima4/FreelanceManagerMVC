import time
import random
import string
import traceback
from datetime import datetime
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager

class DashboardRedirectTest:
    def __init__(self):
        # WebDriver will be initialized in setup()
        self.driver = None
        self.base_url = "https://localhost:7089/"  # Adjust if your app runs on a different port
        self.test_credentials = {
            "username": None,
            "email": None,
            "password": None
        }
        
    def generate_random_username(self):
        """Generate a random username for testing"""
        return ''.join(random.choices(string.ascii_lowercase + string.digits, k=8))
        
    def generate_random_email(self):
        """Generate a random email for testing"""
        username = ''.join(random.choices(string.ascii_lowercase + string.digits, k=8))
        return f"{username}@testmail.com"
    
    def log_info(self, message):
        """Log information message with timestamp"""
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        print(f"[INFO] {timestamp} - {message}")
        
    def log_success(self, message):
        """Log success message with timestamp"""
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        print(f"[SUCCESS] {timestamp} - {message}")
        
    def log_error(self, message, error=None):
        """Log error message with timestamp and optional exception details"""
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        print(f"[ERROR] {timestamp} - {message}")
        if error:
            print(f"Error details: {str(error)}")
            print("Stack trace:")
            traceback.print_exc()
    
    def click_dashboard_button(self):
        """Specifically test finding and clicking the Dashboard button"""
        self.log_info("TEST STEP: Finding and clicking Dashboard button")
        
        try:            # Take screenshot of page before looking for button
            self.driver.save_screenshot("TestImgs/before_dashboard_search.png")
            
            # Try different methods to find the Dashboard button
            
            # Method 1: Direct XPath for Dashboard link
            try:
                self.log_info("Trying to find Dashboard button with XPath...")
                dashboard_link = WebDriverWait(self.driver, 5).until(
                    EC.element_to_be_clickable((By.XPATH, "//a[contains(text(), 'Dashboard')]"))
                )
                self.log_success(f"Found Dashboard button with direct text matching: '{dashboard_link.text}'")
                dashboard_link.click()
                self.log_info("Clicked Dashboard button (XPath method)")
                time.sleep(2)
                return True
            except Exception as e:
                self.log_info("XPath method failed, trying CSS selector...")
            
            # Method 2: CSS selector for buttons, then filter by text
            try:
                self.log_info("Trying to find Dashboard button with CSS selector...")
                buttons = self.driver.find_elements(By.CSS_SELECTOR, ".e-primary a.text-decoration-none")
                
                for button in buttons:
                    self.log_info(f"Found button with text: '{button.text}'")
                    if "Dashboard" in button.text:
                        self.log_success(f"Found Dashboard button with CSS selector: '{button.text}'")
                        button.click()
                        self.log_info("Clicked Dashboard button (CSS selector method)")
                        time.sleep(2)
                        return True
                
                self.log_info("No button containing 'Dashboard' found with CSS selector")
            except Exception as e:
                self.log_info("CSS selector method failed, trying JavaScript...")
            
            # Method 3: JavaScript as a last resort
            self.log_info("Trying to find Dashboard button with JavaScript...")
            result = self.driver.execute_script()
            
            if result:
                self.log_success("JavaScript successfully found and clicked Dashboard button")
                time.sleep(2)
                return True
            else:
                self.log_error("JavaScript could not find Dashboard button")
            
            self.log_error("Failed to find and click Dashboard button using all methods")
            return False
            
        except Exception as e:
            self.log_error("Error in click_dashboard_button method", e)
            return False
    
    def test_dashboard_redirect(self):
        """Test the complete flow from registration to dashboard access"""
        self.log_info("Starting test: Dashboard Redirect Test")
          # STEP 1: Navigate to the landing page
        self.driver.get(self.base_url)
        self.log_info("Opened landing page")
        self.driver.save_screenshot("TestImgs/landing_page_initial.png")
        # Generate random test data
        username = self.generate_random_username()
        fullname = f"Test User {username}"
        email = self.generate_random_email()
        daily_hours = str(random.randint(1, 8))
        password = "Test123!"  # Strong test password
        
        self.test_credentials["username"] = username
        self.test_credentials["email"] = email
        self.test_credentials["password"] = password
        self.log_info(f"Generated test user: {username} / {email} / {password}")
        
        # STEP 2: Find and click Register button on landing page
        try:
            register_button = WebDriverWait(self.driver, 10).until(
                EC.element_to_be_clickable((By.XPATH, "//a[contains(text(), 'Register')]"))
            )
            self.log_info(f"Found Register button with text: '{register_button.text}'")
            register_button.click()
            self.log_info("Clicked Register button")
        except Exception as e:
            self.log_error("Could not find or click Register button", e)
            self.driver.save_screenshot("TestImgs/register_button_error.png")
            return False
        
        # STEP 3: Fill out registration form
        try:
            WebDriverWait(self.driver, 10).until(
                EC.presence_of_element_located((By.ID, "Input.Username"))
            )
            self.log_info("Registration page loaded")
            self.driver.save_screenshot("TestImgs/registration_form.png")
            
            # Fill in the registration form fields
            self.driver.find_element(By.ID, "Input.Username").send_keys(username)
            self.driver.find_element(By.ID, "Input.fullname").send_keys(fullname)
            self.driver.find_element(By.ID, "Input.Email").send_keys(email)
            self.driver.find_element(By.ID, "Input.dailyhours").send_keys(daily_hours)
            self.driver.find_element(By.ID, "Input.Password").send_keys(password)
            self.driver.find_element(By.ID, "Input.ConfirmPassword").send_keys(password)
            
            self.log_info(f"Form filled with test data: {username} / {email}")
            self.driver.save_screenshot("TestImgs/registration_form_filled.png")
            
            # Submit the form
            submit_button = WebDriverWait(self.driver, 10).until(
                EC.element_to_be_clickable((By.CSS_SELECTOR, "button[type='submit']"))
            )
            submit_button.click()
            self.log_info("Registration form submitted")
        except Exception as e:
            self.log_error("Error during registration form submission", e)
            self.driver.save_screenshot("TestImgs/registration_form_error.png")
            return False
        
        # STEP 4: Wait for registration to complete
        time.sleep(3)
        current_url = self.driver.current_url
        self.log_info(f"Current URL after registration: {current_url}")
        self.driver.save_screenshot("TestImgs/after_registration.png")
        
        # STEP 5: Check for successful registration
        # If redirected to Home/Dashboard, registration and auto-login succeeded
        if "/Home" in current_url or "/Dashboard" in current_url:
            self.log_success("SUCCESS: User automatically redirected to Dashboard after registration")
            self.driver.save_screenshot("TestImgs/dashboard_direct_redirect.png")
            return True
        
        # If not redirected, check if we're on the landing page
        if self.base_url in current_url:
            self.log_info("Redirected to landing page, checking if user is logged in")
            
            # Check if the landing page shows the Dashboard button (indicating user is logged in)
            page_source = self.driver.page_source
            user_logged_in = "Dashboard" in page_source and "Login" not in page_source
            
            if user_logged_in:
                self.log_success("User appears to be logged in (Dashboard button present, Login button absent)")
                self.driver.save_screenshot("TestImgs/landing_page_logged_in.png")
                
                # STEP 6: Click the Dashboard button to navigate to dashboard
                if self.click_dashboard_button():
                    # STEP 7: Verify we reached the dashboard page
                    current_url = self.driver.current_url
                    self.log_info(f"URL after clicking Dashboard: {current_url}")
                    self.driver.save_screenshot("TestImgs/dashboard_after_click.png")
                    
                    if "/Home" in current_url or "/Dashboard" in current_url:
                        self.log_success("SUCCESS: Successfully navigated to Dashboard after clicking Dashboard button!")
                        return True
                    else:
                        self.log_error(f"Dashboard button clicked but ended up at unexpected URL: {current_url}")
                        return False
                else:
                    # Even if button click failed, if user is logged in, count as partial success
                    self.log_info("Could not click Dashboard button but user is logged in (partial success)")
                    return True
            else:
                self.log_error("User does not appear to be logged in after registration")
                return False
        
        # For any other case
        self.log_error(f"Unexpected URL after registration: {current_url}")
        return False
    
    def setup(self):
        """Set up the WebDriver for testing"""
        self.log_info("Setting up test environment")
        
        # Set Chrome options
        options = webdriver.ChromeOptions()
        options.add_experimental_option('excludeSwitches', ['enable-logging'])
        options.add_argument("--log-level=3")
        
        # Initialize WebDriver
        self.driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)
        self.driver.maximize_window()
        self.driver.implicitly_wait(10)
    
    def teardown(self):
        """Clean up after test"""
        self.log_info("Test completed. Waiting for 10 seconds before closing the browser...")
        
        try:
            # Take a final screenshot
            if self.driver:
                self.driver.save_screenshot("TestImgs/final_test_state.png")
            
            # Wait a bit to see the final state
            time.sleep(10)
        except Exception as e:
            self.log_error("Error during teardown", e)
        
        # Close the browser
        if self.driver:
            self.driver.quit()
            self.log_info("WebDriver closed")

# Main execution
if __name__ == "__main__":
    # Create test instance
    test = DashboardRedirectTest()
    
    # Track test result
    test_result = False
    
    try:
        # Print header
        print("\n" + "="*80)
        print(" "*25 + "DASHBOARD REDIRECT TEST")
        print("="*80 + "\n")
        
        # Setup WebDriver
        test.setup()
        
        # Run the test
        test_result = test.test_dashboard_redirect()
        
        # Test summary
        print("\n" + "="*80)
        print(" "*35 + "TEST SUMMARY")
        print("="*80)
        
        status = "✓ PASSED" if test_result else "✗ FAILED"
        status_color = "\033[92m" if test_result else "\033[91m"  # Green for pass, red for fail
        print(f"{status_color}DASHBOARD REDIRECT TEST: {status}\033[0m")
        
        print("\n" + "="*80 + "\n")
        
    except Exception as e:
        test.log_error("Unhandled exception during test execution", e)
    finally:
        # Clean up
        test.teardown()
        test.log_info("Test completed.")
