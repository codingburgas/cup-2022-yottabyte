import 'package:dji/dji.dart';
import 'package:dji/flight.dart';
import 'package:dji/messages.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:developer' as developer;
import 'page/map.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        // This is the theme of your application.
        //
        // Try running your application with "flutter run". You'll see the
        // application has a blue toolbar. Then, without quitting the app, try
        // changing the primarySwatch below to Colors.green and then invoke
        // "hot reload" (press "r" in the console where you ran "flutter run",
        // or simply save your changes to "hot reload" in a Flutter IDE).
        // Notice that the counter didn't reset back to zero; the application
        // is not restarted.
        primarySwatch: Colors.blue,
      ),
      home: const MyHomePage(title: 'Flutter Demo Home Page'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  // This widget is the home page of your application. It is stateful, meaning
  // that it has a State object (defined below) that contains fields that affect
  // how it looks.

  // This class is the configuration for the state. It holds the values (in this
  // case the title) provided by the parent (in this case the App widget) and
  // used by the build method of the State. Fields in a Widget subclass are
  // always marked "final".

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> implements DjiFlutterApi {
  String _platformVersion = 'Unknown';
  String _droneStatus = 'Disconnected';
  String _droneError = '';
  String _droneBatteryPercent = '0';
  String _droneAltitude = '0.0';
  double _droneLatitude = 0.0;
  double _droneLongitude = 0.0;
  String _droneSpeed = '0.0';
  String _droneRoll = '0.0';
  String _dronePitch = '0.0';
  String _droneYaw = '0.0';

  double _leftStickHorizontal = 0.0;
  double _leftStickVertical = 0.0;
  double _rightStickHorizontal = 0.0;
  double _rightStickVertical = 0.0;

  double _virtualStickPitch = 0.0;
  double _virtualStickRoll = 0.0;
  double _virtualStickYaw = 0.0;
  double _virtualStickVerticalThrottle = 0.0;

  double _gimbalPitchInDegrees = 0.0;

  FlightLocation? droneHomeLocation;
  
  @override
  void initState() {
    super.initState();

    DjiFlutterApi.setup(this);

    _getPlatformVersion();
  }

  @override
  void setStatus(Drone drone) async {
    setState(() {
      _droneStatus = drone.status ?? 'Disconnected';
      _droneError = drone.error ?? '';
      _droneAltitude = drone.altitude?.toStringAsFixed(2) ?? '0.0';
      _droneBatteryPercent = drone.batteryPercent?.toStringAsFixed(0) ?? '0';
      _droneLatitude = drone.latitude ?? 0.0;
      _droneLongitude = drone.longitude ?? 0.0;
      _droneSpeed = drone.speed?.toStringAsFixed(2) ?? '0.0';
      _droneRoll = drone.roll?.toStringAsFixed(3) ?? '0.0';
      _dronePitch = drone.pitch?.toStringAsFixed(3) ?? '0.0';
      _droneYaw = drone.yaw?.toStringAsFixed(3) ?? '0.0';
    });

    // Setting the initial drone location as the home location of the drone.
    if (droneHomeLocation == null &&
        drone.latitude != null &&
        drone.longitude != null &&
        drone.altitude != null) {
      droneHomeLocation = FlightLocation(
          latitude: drone.latitude!,
          longitude: drone.longitude!,
          altitude: drone.altitude!);
    }
  }

  Future<void> _getPlatformVersion() async {
    String platformVersion;

    // Platform messages may fail, so we use a try/catch PlatformException.
    // We also handle the message potentially returning null.
    try {
      platformVersion = await Dji.platformVersion ?? 'Unknown platform version';
    } on PlatformException {
      platformVersion = 'Failed to get platform version.';
    }

    // If the widget was removed from the tree while the asynchronous platform
    // message was in flight, we want to discard the reply rather than calling
    // setState to update our non-existent appearance.
    if (!mounted) return;

    setState(() {
      _platformVersion = platformVersion;
    });
  }

  Future<void> _registerApp() async {
    try {
      developer.log(
        'registerApp requested',
        name: kLogKindDjiFlutterPlugin,
      );
      await Dji.registerApp();
    } on PlatformException catch (e) {
      developer.log(
        'registerApp PlatformException Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    } catch (e) {
      developer.log(
        'registerApp Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    }
  }

  Future<void> _connectDrone() async {
    try {
      developer.log(
        'connectDrone requested',
        name: kLogKindDjiFlutterPlugin,
      );
      await Dji.connectDrone();
    } on PlatformException catch (e) {
      developer.log(
        'connectDrone PlatformException Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    } catch (e) {
      developer.log(
        'connectDrone Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    }
  }

  Future<void> _delegateDrone() async {
    try {
      await Dji.delegateDrone();
      developer.log(
        'delegateDrone succeeded',
        name: kLogKindDjiFlutterPlugin,
      );
    } on PlatformException catch (e) {
      developer.log(
        'delegateDrone PlatformException Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    } catch (e) {
      developer.log(
        'delegateDrone Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    }
  }

  Future<void> _takeOff() async {
    try {
      await Dji.takeOff();
      developer.log(
        'Takeoff succeeded',
        name: kLogKindDjiFlutterPlugin,
      );
    } on PlatformException catch (e) {
      developer.log(
        'Takeoff PlatformException Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    } catch (e) {
      developer.log(
        'Takeoff Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    }
  }

  Future<void> _land() async {
    try {
      await Dji.land();
      developer.log(
        'Land succeeded',
        name: kLogKindDjiFlutterPlugin,
      );
    } on PlatformException catch (e) {
      developer.log(
        'Land PlatformException Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    } catch (e) {
      developer.log(
        'Land Error',
        error: e,
        name: kLogKindDjiFlutterPlugin,
      );
    }
  }
    
  @override
  Widget build(BuildContext context) {
    // This method is rerun every time setState is called, for instance as done
    // by the _incrementCounter method above.
    //
    // The Flutter framework has been optimized to make rerunning build methods
    // fast, so that you can just rebuild anything that needs updating rather
    // than having to individually change instances of widgets.
    return Scaffold(
      appBar: AppBar(
        // Here we take the value from the MyHomePage object that was created by
        // the App.build method, and use it to set our appbar title.
        title: Container(
          child: Text('$_droneBatteryPercent% $_droneStatus'),
        )
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            // make ui for registerring app and connecting to drone
            ButtonBar(
              alignment: MainAxisAlignment.center,
              children: <Widget>[
                Column(
                  children: [
                    Text('$_platformVersion'),
                    Text('$_droneError'),
                  ]),
                Column(
                  children: [
                    Text('Latitude: $_droneLatitude, Longitude: $_droneLongitude'),
                    Text('Altitude: $_droneAltitude'),
                    Text('Speed: $_droneSpeed'),
                    Text('Roll: $_droneRoll'),
                    Text('Pitch: $_dronePitch'),
                    Text('Yaw: $_droneYaw'),
                  ]),
                Column(
                  children: [
                    ElevatedButton(
                      onPressed: _registerApp,
                      child: const Text('Register App'),
                    ),
                    ElevatedButton(
                      onPressed: _connectDrone,
                      child: const Text('Connect to Drone'),
                    ),
                    // delegate button
                    ElevatedButton(
                      onPressed: _delegateDrone,
                      child: const Text('Delegate Drone'),
                    ),
                    ElevatedButton(
                      onPressed: _takeOff, 
                      child: const Text('Take off')
                    ),
                    ElevatedButton(
                      onPressed: _land,
                      child: const Text('Land'))
                  ],
                ),
                
              ],
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          Navigator.push(
            context,
            MaterialPageRoute(builder: (context) => const MapPage()),
          );
        },
        tooltip: 'Next page',
        child: const Icon(Icons.arrow_forward),
      ), // This trailing comma makes auto-formatting nicer for build methods.
    );
  } 

  @override
  void sendVideo(Stream stream) {
    // TODO: implement sendVideo
  }
}
