import React, { useEffect, useState } from "react";
import {
  StyleSheet,
  View,
  Dimensions,
  Alert,
  StatusBar,
  Text,
  Image,
} from "react-native";
import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import MapView from "react-native-maps";
import * as Location from "expo-location";
import Svg, { Path, Circle } from "react-native-svg";

const SERVER_ENDPOINT = "//yottabyte-server-test.azurewebsites.net";
const API_ENDPOINT = SERVER_ENDPOINT + "/api";

<StatusBar translucent backgroundColor="transparent" />;
const Stack = createBottomTabNavigator();

function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator
        initialRouteName="Feed"
        screenOptions={{
          tabBarActiveTintColor: "#e91e63",
          tabBarStyle: { height: 112 },
        }}
      >
        <Stack.Screen
          name="Map"
          component={Map}
          options={{
            tabBarLabel: " ",
            tabBarIcon: ({ color, size }) => (
              <Image
                style={{
                  width: 35,
                  height: 44,
                }}
                source={require("./assets/images/mapIconSelected.png")}
              ></Image>
            ),
            headerShown: false,
          }}
        />
        <Stack.Screen
          name="  "
          component={Events}
          options={{
            tabBarLabel: " ",
            tabBarIcon: ({ color, size }) => (
              <Image
                style={{
                  width: 45,
                  height: 45,
                }}
                source={require("./assets/images/searchIconSelected.png")}
              ></Image>
            ),
            headerShown: false,
          }}
        />
        <Stack.Screen
          name="   "
          component={Notifications}
          options={{ headerShown: false }}
        />
        <Stack.Screen
          name="    "
          component={User}
          options={{ headerShown: false }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

function Map() {
  const [location, setLocation] = useState(null);
  const [userLocationLat, setUserLocationLat] = useState(0);
  const [userLocationLong, setUserLocationLong] = useState(0);

  useEffect(() => {
    (async () => {
      const { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== "granted") {
        Alert.alert(
          "No location access",
          "Allow Yottabyte to access your location or turn on the location of the device",
          [
            {
              text: "Cancel",
              style: "cancel",
            },
            { text: "Open settings", onPress: () => Linking.openSettings() },
          ]
        );
      }

      const location = await Location.getLastKnownPositionAsync({
        accuracy: 6,
        distanceInterval: 1,
        timeInterval: 1000,
      });
      setLocation(location);
      if (location == null && userLocationLat != -80) {
        setUserLocationLat(-80);
        setUserLocationLong(-176);
      } else if (userLocationLat != location.coords.latitude) {
        setUserLocationLat(location.coords.latitude);
        setUserLocationLong(location.coords.longitude);
      }
    })();
  }, []);

  return (
    <View style={styles.container}>
      <MapView
        style={styles.map}
        mapType="satellite"
        paddingAdjustmentBehavior="automatic"
        toolbarEnabled={false}
        userLocationPriority="high"
        userLocationFastestInterval={1000}
        userLocationUpdateInterval={1000}
        showsUserLocation={true}
        showsMyLocationButton={true}
        initialRegion={{
          latitude: 42.4991874040583,
          longitude: 27.46135711669922,
          latitudeDelta: 0.0922,
          longitudeDelta: 0.0421,
        }}
      />
    </View>
  );
}

function Events({ navigation }) {
  const [eventData, setEventData] = useState(null);
  useEffect(()=>{
    const onEventsEnter = navigation.addListener('focus', ()=>{
      console.log("fetch data for events");
      fetchEvents().then((eventsDataJSON) => {
        setEventData(eventsDataJSON);
        console.log(eventData);
      });
    });
    return onEventsEnter;
  }, [navigation]);

  return (
    <>
      <View style={styles.rectangle}></View>
    </>
  );

}

function Notifications() {
  return (
    <>
      <Text>testink...</Text>
    </>
  );
}

function User() {
  return (
    <>
      <Text>testink...</Text>
    </>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    width: Dimensions.get("window").width,
    height: Dimensions.get("screen").height,
  },

  eventContainer: {
    height: "128px",
    width: "128px",
    backgroundColor: 'salmon',
    position: 'absolute', 
    zIndex: 99,
    top: '0%',
    left: '0%',
  },
});

function toURLEncoded(data) {
  return Object.keys(data)
      .map(
          (key) =>
              encodeURIComponent(key) + "=" + encodeURIComponent(data[key])
      )
      .join("&");
}

async function fetchAPI(
  endpoint,
  data = {},
  headers = {},
  method = "GET"
) {
  let response = await fetch("https:" + API_ENDPOINT + endpoint, {
      method,
      mode: "cors",
      headers: {
          "Content-Type": "application/form-data",
          ...headers,
      },
      body: method === "GET" ? null : toURLEncoded(data),
  });
  return response.json();
}

async function fetchEvents(token) {
  return new Promise((res, rej) => {
      fetchAPI(
          "/events/"
      )
          .then(res)
          .catch(rej);
  });
}

export default App;
