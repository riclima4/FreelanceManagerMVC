import subprocess
import sys

def install_requirements():
    """Install required packages for UI testing."""
    print("Installing required packages for UI testing...")
    
    requirements = [
        'selenium',
        'webdriver-manager'
    ]
    
    for package in requirements:
        print(f"Installing {package}...")
        subprocess.check_call([sys.executable, '-m', 'pip', 'install', package])
    
    print("\nAll required packages installed successfully!")
    print("\nYou can now run the UI test with: python UiTest.py")

if __name__ == "__main__":
    install_requirements()
