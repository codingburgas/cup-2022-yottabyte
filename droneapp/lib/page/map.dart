

import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:geolocator/geolocator.dart';
import 'package:mapbox_gl/mapbox_gl.dart';

const String ACCESS_TOKEN = "pk.eyJ1Ijoic3NpdmFub3YxOSIsImEiOiJjbGdtZWNjdDcwNTF2M21vaHd5YzdocXgwIn0.MgDy2raKr-KPjl-27ZD83Q";
class MapPage extends StatefulWidget {
  const MapPage({Key? key}) : super(key: key);

  @override
  State<MapPage> createState() => _MapPageState();
}

class _MapPageState extends State<MapPage>  {
  List<LatLng> latlngList = <LatLng>[];
  LatLng currentLocation = const LatLng(42.491403, 27.482109);
  MapboxMapController? controller;

  Future<Position> _determinePosition() async {
    bool serviceEnabled;
    LocationPermission permission;

    // Test if location services are enabled.
    serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      // Location services are not enabled don't continue
      // accessing the position and request users of the 
      // App to enable the location services.
      return Future.error('Location services are disabled.');
    }

    permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission == LocationPermission.denied) {
        // Permissions are denied, next time you could try
        // requesting permissions again (this is also where
        // Android's shouldShowRequestPermissionRationale 
        // returned true. According to Android guidelines
        // your App should show an explanatory UI now.
        return Future.error('Location permissions are denied');
      }
    }
    
    if (permission == LocationPermission.deniedForever) {
      // Permissions are denied forever, handle appropriately. 
      return Future.error(
        'Location permissions are permanently denied, we cannot request permissions.');
    } 

    // When we reach here, permissions are granted and we can
    // continue accessing the position of the device.
    return await Geolocator.getCurrentPosition();
  }

  Future<void> _getCurrentLocation() async {
    var pos = await _determinePosition();
    setState(() {
      currentLocation = LatLng(pos.latitude, pos.longitude);
    });
    
  }

  @override
  void initState() {
    _getCurrentLocation();
    super.initState();
  }

  void _addMarker(LatLng latLng) async {
    //var byteData = await rootBundle.load("images/poi.png");
    //var markerImage = byteData.buffer.asUint8List();
    controller?.addCircle(CircleOptions(
      geometry: latLng,
      circleRadius: 10,
      circleColor: "#FF0000",
      circleOpacity: 0.5,
      circleStrokeWidth: 2,
      circleStrokeColor: "#FF0000",
    ));

    if (latlngList.isNotEmpty) {
       controller?.addLine(LineOptions(
        geometry: [latlngList.last, latLng],
        lineColor: "#FF0000",
        lineWidth: 2,
        lineOpacity: 0.5,
      ));
    }

    setState(() {
      latlngList.add(latLng);
    });
  }

  void _addController(MapboxMapController controller) {
    setState(() {
      this.controller = controller;
    });
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Plan your path"),
      ),
      body: Center(
        child: MapboxMap(
          initialCameraPosition: CameraPosition(
            target: currentLocation,
            zoom: 13,
          ),
          myLocationEnabled: true,
          myLocationTrackingMode: MyLocationTrackingMode.Tracking,
          onMapClick: (point, coordinates) => _addMarker(coordinates),
          onMapCreated: (controller) => _addController(controller),
          accessToken: ACCESS_TOKEN,
          styleString: "mapbox://styles/ssivanov19/clgmedn2e00b601qqhspmh51o"
        ),
      ),
    );
  }
}
/*
  @override
  void initState() {
    _getCurrentLocation();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Plan your path"),
      ),
      body: Center(
        child: FlutterMap(
          options: MapOptions(
            center: currentLocation,
            zoom: 12.0,
          ),
          layers: [
            TileLayerOptions(
              urlTemplate: "https://api.mapbox.com/styles/v1/ssivanov19/clgmedn2e00b601qqhspmh51o/tiles/256/{z}/{x}/{y}@2x?access_token=pk.eyJ1Ijoic3NpdmFub3YxOSIsImEiOiJjbGdtZWNjdDcwNTF2M21vaHd5YzdocXgwIn0.MgDy2raKr-KPjl-27ZD83Q",
              additionalOptions: {
                'accessToken': 'pk.eyJ1Ijoic3NpdmFub3YxOSIsImEiOiJjbGdtZWNjdDcwNTF2M21vaHd5YzdocXgwIn0.MgDy2raKr-KPjl-27ZD83Q',
                'id': 'clgmedn2e00b601qqhspmh51o',
              },
            ),
            MarkerLayerOptions(
              markers: [
                Marker(
                  width: 50.0,
                  height: 50.0,
                  point: LatLng(31.050478, -7.931633),
                  builder: (ctx) => Container(
                    child: Image.asset(
                      "use an image for marker",
                    ),
                  ),
                )
              ],
            ),
            PolylineLayerOptions(polylines: [
              Polyline(
                points: latlngList,
                // isDotted: true,
                color: const Color(0xFF669DF6),
                strokeWidth: 3.0,
                borderColor: const Color(0xFF1967D2),
                borderStrokeWidth: 0.1,
              )
            ])
          ],
        ),
      ),
    );
  }*/
