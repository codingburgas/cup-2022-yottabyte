import React, { useEffect, useState } from "react";
import {
  StyleSheet,
  View,
  Dimensions,
  Alert,
  StatusBar,
  Text,
  Image,
  SafeAreaView,
  ScrollView,
  TouchableOpacity,
} from "react-native";
import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import MapView from "react-native-maps";
import { Marker } from "react-native-maps";
import * as Location from "expo-location";
import Svg, { Path, Circle } from "react-native-svg";
import MaterialCommunityIcons from "react-native-vector-icons/MaterialCommunityIcons";

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
          tabBarActiveTintColor: "#6cc3ec",
          tabBarStyle: { height: 102 },
        }}
      >
        <Stack.Screen
          name="Map"
          component={Map}
          options={{
            tabBarLabel: " ",
            tabBarIcon: ({ color }) => (
              <MaterialCommunityIcons
                name="map-marker"
                color={color}
                size={55}
              />
            ),
            headerShown: false,
          }}
        />
        <Stack.Screen
          name="  "
          component={Events}
          options={{
            tabBarLabel: " ",
            tabBarIcon: ({ color }) => (
              <MaterialCommunityIcons
                name="format-list-bulleted"
                color={color}
                size={55}
              />
            ),
            headerShown: false,
          }}
        />
        <Stack.Screen
          name="   "
          component={Notifications}
          options={{
            tabBarLabel: " ",
            tabBarIcon: ({ color }) => (
              <MaterialCommunityIcons
                name="bell-outline"
                color={color}
                size={55}
              />
            ),
            headerShown: false,
          }}
        />
        <Stack.Screen
          name="    "
          component={User}
          options={{
            tabBarLabel: " ",
            tabBarIcon: ({ color }) => (
              <MaterialCommunityIcons
                name="face-recognition"
                color={color}
                size={50}
              />
            ),
            headerShown: false,
          }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

function Map({ navigation }) {
  const [location, setLocation] = useState(null);
  const [userLocationLat, setUserLocationLat] = useState(0);
  const [userLocationLong, setUserLocationLong] = useState(0);
  let markerCoordsLat = 0;
  let markerCoordsLong = 0;

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

  const [eventData, setEventData] = useState(null);
  useEffect(() => {
    const onEventsEnter = navigation.addListener("focus", () => {
      console.log("fetch data for events");
      fetchEvents().then((eventsDataJSON) => {
        setEventData(eventsDataJSON);
        // markerCoordsLat = eventData[0].lat;
        // console.log(eventData[0].lat);
        // markerCoordsLong = eventData[0].long;
      });
    });
    return onEventsEnter;
  }, [navigation]);

  let markerList = [];
  if (eventData != null) {
    eventData.forEach((evtDat) => {
      markerList.push(
        <Marker
          coordinate={{
            latitude: parseFloat(evtDat.lat),
            longitude: parseFloat(evtDat.long),
          }}
        >
          <Image
            source={require("./assets/images/markerImage.png")}
            style={{ height: 20, width: 20 }}
          />
        </Marker>
      );
    });
  }

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
      >
        {markerList}
      </MapView>
    </View>
  );
}

function Events({ navigation }) {
  const [eventData, setEventData] = useState(null);
  useEffect(() => {
    const onEventsEnter = navigation.addListener("focus", () => {
      fetchEvents().then((eventsDataJSON) => {
        setEventData(eventsDataJSON);
      });
    });
    return onEventsEnter;
  }, [navigation]);

  let events = [];

  if (eventData != null) {
    eventData.forEach((event) => {
      let name = event.location.split(",");
      let time = event.startTime.split("T");
      events.push(
        <View style={styles.eventContainer}>
          <Image
            source={{
              uri: `${event.imageURL}`,
            }}
            style={styles.eventImage}
          />
          <Text style={styles.eventName}>{name[2].substr(1, 12)}</Text>
          <MaterialCommunityIcons
            name="map-marker-outline"
            color="#B2B1B6"
            size={13}
            style={styles.eventIconLocation}
          />
          <Text style={styles.eventLocation}>
            {event.location.substr(0, 30)}
          </Text>
          <MaterialCommunityIcons
            name="clock-time-two-outline"
            color="#B2B1B6"
            size={13}
            style={styles.eventIconTime}
          />
          <Text style={styles.eventTime}>{time[1]}</Text>
          <TouchableOpacity style={styles.eventButton}>
            <Text style={styles.eventButtonText}>More Info</Text>
          </TouchableOpacity>
        </View>
      );
    });
  }

  return (
    <SafeAreaView>
      <ScrollView>{events}</ScrollView>
    </SafeAreaView>
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
    height: 192,
    width: "91%",
    backgroundColor: "white",
    borderRadius: 27,
    marginTop: 34,
    marginBottom: 10,
    left: "4.5%",
    shadowColor: "#9D9D9D",
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.25,
    shadowRadius: 2.22,

    elevation: 25,
  },

  eventImage: {
    width: "30%",
    height: 158,
    top: 17,
    left: "4%",
    borderRadius: 22,
    position: "absolute",
  },

  eventName: {
    fontSize: 28,
    top: 23,
    left: "42%",
    color: "#00103A",
  },

  eventLocation: {
    fontSize: 11,
    top: 33,
    left: "46%",
    color: "#B2B1B6",
  },

  eventTime: {
    fontSize: 11,
    top: 43,
    left: "46%",
    color: "#B2B1B6",
  },

  eventIconLocation: {
    position: "absolute",
    top: 76,
    left: "42%",
  },

  eventIconTime: {
    position: "absolute",
    top: 102,
    left: "42%",
  },

  eventButton: {
    height: 41,
    width: "47%",
    borderRadius: 45,
    backgroundColor: "#245BF5",
    top: 61,
    left: "42%",
  },

  eventButtonText: {
    fontSize: 16,
    top: 8,
    left: "25%",
    color: "white",
    fontWeight: "bold",
  },
});

function toURLEncoded(data) {
  return Object.keys(data)
    .map((key) => encodeURIComponent(key) + "=" + encodeURIComponent(data[key]))
    .join("&");
}

async function fetchAPI(endpoint, data = {}, headers = {}, method = "GET") {
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
    fetchAPI("/events/").then(res).catch(rej);
  });
}

export default App;
