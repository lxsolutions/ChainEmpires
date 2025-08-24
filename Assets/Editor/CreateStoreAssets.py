


import os
import sys
from PIL import Image, ImageDraw, ImageFont

def create_store_directory():
    """Create the store directory structure if it doesn't exist"""
    base_dir = "Assets/Store"
    screenshots_dir = os.path.join(base_dir, "Screenshots")

    os.makedirs(base_dir, exist_ok=True)
    os.makedirs(screenshots_dir, exist_ok=True)

def create_placeholder_icon(path, size=(512, 512), text="CE"):
    """Create a placeholder icon with the given text"""
    img = Image.new('RGB', size, color='black')
    draw = ImageDraw.Draw(img)

    try:
        font = ImageFont.truetype("arial.ttf", size=size[0] // 3)
    except IOError:
        font = ImageFont.load_default()

    # Use textbbox for PIL versions that support it
    try:
        bbox = draw.textbbox((0, 0), text, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]
    except AttributeError:
        # Fallback for older PIL versions
        text_width, text_height = draw.textsize(text, font=font)

    position = ((size[0] - text_width) / 2, (size[1] - text_height) / 2)
    draw.text(position, text, fill="white", font=font)
    img.save(path)

def create_placeholder_screenshot(path, size=(1920, 1080), scene_name="Scene"):
    """Create a placeholder screenshot with gradient background and text"""
    img = Image.new('RGB', size)

    # Create gradient
    pixels = []
    for y in range(size[1]):
        t = y / size[1]
        color = (
            int(255 * (1 - t)),
            int(255 * t),
            0,
            255
        )
        row = [color] * size[0]
        pixels.extend(row)

    img.putdata(pixels)
    draw = ImageDraw.Draw(img)

    try:
        font = ImageFont.truetype("arial.ttf", size=size[1] // 10)
    except IOError:
        font = ImageFont.load_default()

    # Use textbbox for PIL versions that support it
    try:
        bbox = draw.textbbox((0, 0), scene_name, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]
    except AttributeError:
        # Fallback for older PIL versions
        text_width, text_height = draw.textsize(scene_name, font=font)

    position = ((size[0] - text_width) / 2, (size[1] - text_height) / 2)

    # Draw semi-transparent text
    shadow_color = "black"
    shadow_position = (position[0] + 3, position[1] + 3)
    draw.text(shadow_position, scene_name, font=font, fill=shadow_color)

    draw.text(position, scene_name, font=font, fill="white")
    img.save(path)

def create_store_description():
    """Create the store description file"""
    description_path = "Assets/Store/store_description.md"

    with open(description_path, 'w') as f:
        f.write("""# Chain Empires Store Description

## Game Overview
Chain Empires is a revolutionary 3rd-person mobile strategy game that combines elements from Age of Empires, Civilization, Diablo, and Starcraft with modern P2E economics. Players build their empires through resource management, unit training, base upgrades, and strategic combat.

## Key Features
- **Turn-based PvP/PvE Combat**: Strategic battles against AI and other players
- **Base Building & Upgrades**: Customize your empire with unique structures and defenses
- **Resource Management**: Collect and allocate resources for growth and expansion
- **Adaptive AI Waves**: Intelligent enemy behavior with learning and diplomacy systems
- **Narrative Echo Realms**: Immersive story-driven gameplay with branching paths
- **AR Buffs & NFT Cosmetics**: Augmented reality enhancements and customizable items
- **Solana Web3 Integration**: True ownership of in-game assets via blockchain technology

## Gameplay Screenshots
![Screenshot 1](Screenshots/screenshot_1.png)
![Screenshot 2](Screenshots/screenshot_2.png)
![Screenshot 3](Screenshots/screenshot_3.png)

## System Requirements
- **Android**: Version 8.0 or higher, 4GB RAM recommended
- **iOS**: iOS 13 or higher, iPhone 7 or newer recommended

## Contact Information
For support and inquiries: contact@chainempires.com
""")

def main():
    """Main function to generate all store assets"""
    create_store_directory()

    # Create placeholder icons
    icon_sizes = [
        ("icon_512x512.png", (512, 512)),
        ("icon_180x180.png", (180, 180)),
        ("icon_72x72.png", (72, 72))
    ]

    for icon_name, size in icon_sizes:
        create_placeholder_icon(os.path.join("Assets/Store", icon_name), size)

    # Create placeholder screenshots
    for i in range(1, 6):
        screenshot_path = os.path.join("Assets/Store/Screenshots", f"screenshot_{i}.png")
        create_placeholder_screenshot(screenshot_path, scene_name=f"Scene {i}")

    # Create store description
    create_store_description()

    print("Generated all store assets successfully")

if __name__ == "__main__":
    main()

