# Download
@htmlonly
<h2 id='textAndroid'>From here you can download our mobile app for Android:</h2>
<a href='https://github.com/SSIvanov19/maze-game-2021/releases/download/Release/Release.zip'><button>Download our mobile app</button></a>

<h2 id='textIOS'>And here is for IOS:</h2>
<a href='https://github.com/SSIvanov19/maze-game-2021/releases/download/Release/Release.zip'><button>Download our mobile app</button></a>
 <script type="text/javascript">
    let buttonAndroid = document.getElementsByTagName("button")[0];
    let textAndroid = document.getElementById("textAndroid");
    let buttonIOS = document.getElementsByTagName("button")[1];
    let textIOS = document.getElementById("textIOS");

    buttonAndroid.addEventListener("click", function() {
        textAndroid.innerHTML = "Qsha! Thanks for your support!";
    });

    buttonIOS.addEventListener("click", function() {
        textIOS.innerHTML = "Qsha! Thanks for your support!";
    });
 </script>
@endhtmlonly