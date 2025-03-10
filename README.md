# **Game Objects Finder**

![Before finding](before.png)
 ![After finding](after.png)

## **Overview**

Game Objects Finder is a Unity Editor tool designed to streamline object searching. It allows you to **find objects** in the scene based on their tag or attached script, improving efficiency when working in Unity.

Additionally, the package includes **custom attributes** to make the Unity Inspector more user-friendly and intuitive.

## **Installation**

1.  Download the ZIP file.
2.  Extract it and import into your Unity project.
3.  Add the **GameObjectFinder** prefab to your scene or attach the `GameObjectsFinder` script to any GameObject in your scene.

## **How to Use**

The `GameObjectsFinder` script provides a simple way to search for objects in the scene using the Unity Inspector. You can:

-   **Search by Tag** – Enter a tag name to find all objects with that tag.  
-   **Search by Script** – Enter a script name to find all objects that have that script attached.  
-   Use the **Inspector GUI** to input search parameters and execute searches via dedicated buttons.  

📌 _The script includes detailed inline comments explaining its functionality._ 

## **Demo Scene**

The package includes a **Game Objects Finder Demo** scene to showcase its capabilities:

-   The scene features a plane with 16 spawn points.
-   Selecting the `CubeSpawner` object and pressing a button in the `CubeSpawner` script will spawn 16 cubes at random locations.
-   The spawned cubes have different properties:
    -   Some are tagged as **"Player"**.
    -   Some have the **EmptyDemoScript** attached.
    -   Some are standard cubes without additional components.
-   Selecting the `GameObjectsFinder` object allows you to enter a tag or script name in the Inspector and quickly locate objects accordingly.

## **License**

This project is licensed under the **HardCodeDev License (MIT-based, modified)**. See `LICENSE.txt` for details.

## **Author**

-   **HardCodeDev**
-   [GitHub](https://github.com/HardCodeDev777)
-   [Itch.io](https://hardcodedev.itch.io/)
