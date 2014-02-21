<?php
print_r($_GET);
$url = $_GET["url"];
$checkDomain = "http://o.scdn.co/";

for ($i = 0; $i < 16; $i++)
{
        if($url[$i] != $checkDomain[$i]) die("Get out"); // bite me
}

$jpeg_image = imagecreatefromjpeg($_GET["url"]);
$filename = "art" . $_GET["rand"] . ".jpg";
echo($filename);

imagejpeg($jpeg_image, $filename);
?>