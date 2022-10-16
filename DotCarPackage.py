import shutil
import os
import time

def main():
    cwd = os.getcwd()
    if not os.path.exists(cwd + "\\Builds\\_Distribution"):
        os.makedirs(cwd + "\\Builds\\_Distribution")
    shutil.copy2(cwd + "\\Builds\\Release\\x64\\DotCarDumper.exe", cwd + "\\Builds\\_Distribution\\DotCarDumper.x64.exe")
    shutil.copy2(cwd + "\\Builds\\Release\\x86\\DotCarDumper.exe", cwd + "\\Builds\\_Distribution\\DotCarDumper.x86.exe")
    
    print("Packaging complete")
    time.sleep(3)

if __name__ == "__main__":
    main()
