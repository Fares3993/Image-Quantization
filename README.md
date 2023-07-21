# Image Quantization

A color quantization program in C# is a computer program that reduces the number of distinct colors in an image while attempting to preserve its visual quality. The process of color quantization involves mapping the original colors in an image to a smaller set of representative colors, usually using a technique called clustering. This is commonly done to reduce the memory required to store an image or to simplify its appearance.

#### Here's a general outline of how a color quantization program in C# might work:

## 1- Load the Image: 
The program will first load an image in a supported format (such as PNG, JPEG, BMP, etc.) from a file or any other source.

## 2- Extract Color Information:
For each pixel in the image, the program will extract the RGB (Red, Green, Blue) values representing the color.

## 3- Color Clustering:
The color quantization algorithm comes into play here. There are several algorithms available, but a common one is the k-means clustering algorithm. This algorithm groups similar colors together into clusters and then assigns a representative color (centroid) to each cluster.

## 4- Reduce Color Palette:
After clustering, the program will create a reduced color palette containing the representative colors from the clusters. The number of colors in the palette will be significantly smaller than the original number of colors in the image.

## 5- Remap Image Colors:
Now, the program will remap each pixel's color in the original image to the closest color in the reduced color palette. This step effectively reduces the number of distinct colors in the image.

## 6- Generate Quantized Image:
Finally, the program will create and save a new image with the quantized color palette. This image will have fewer colors and will be visually simplified compared to the original image.

## Additional considerations:
<ul>
<li>Some advanced color quantization algorithms may take into account perceptual color differences to preserve visual quality.</li> 
<li>The program may allow the user to specify the desired number of colors in the reduced palette.</li> 
<li>It may also provide options to choose different color quantization algorithms or adjust parameters for better results.</li> 
</ul>

## Some Screenshots:

<img src ="https://github.com/Fares3993/Image-Quantization/assets/84674642/0b286dc2-2806-466c-a77b-e23ffe3f29b7">
<img src = "https://github.com/Fares3993/Image-Quantization/assets/84674642/04ec54d7-9e35-4925-9e86-4d550a17b179">
<img src = "https://github.com/Fares3993/Image-Quantization/assets/84674642/93f905bb-9c98-4c91-af63-8c44e72b710f">







