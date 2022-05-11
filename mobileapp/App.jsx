import React, { useEffect, useState, useRef } from "react";
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
import RBSheet from "react-native-raw-bottom-sheet";

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
  const refRBSheet = useRef();
  const [modalName, setModalName] = useState(null);
  const [modalImage, setModalImage] = useState(null);
  const [modalLocation, setModalLocation] = useState(null);
  const [modalDate, setModalDate] = useState(null);
  const [modalTime, setModalTime] = useState(null);

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
    eventData.forEach((event) => {
      markerList.push(
        <Marker
          coordinate={{
            latitude: parseFloat(event.lat),
            longitude: parseFloat(event.long),
          }}
          onPress={() => {
            refRBSheet.current.open();
            setModalInfo(event);
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

  const setModalInfo = (event) => {
    let name = event.location.split(",");
    let time = event.startTime.split("T");
    let location;
    console.log(event.location.length);
    location =
      event.location.length > 30
        ? event.location.substr(0, 30) + "..."
        : event.location;
    setModalName(name[2]);
    setModalImage(event.imageURL);
    setModalLocation(location);
    setModalDate(time[0]);
    setModalTime(time[1]);
  };

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
      <RBSheet
        ref={refRBSheet}
        closeOnDragDown={true}
        closeOnPressMask={true}
        height={Dimensions.get("screen").height - 100}
        customStyles={{
          wrapper: {
            backgroundColor: "transparent",
            borderRadius: 100,
          },
          draggableIcon: {
            backgroundColor: "#6D6D6D",
          },
          container: {
            borderRadius: 50,
          },
        }}
      >
        {modalName != null && (
          <>
            <Text style={styles.modalName}>{modalName}</Text>
            <View style={styles.modalImageBox}>
              <Image
                source={{
                  uri: `${modalImage}`,
                }}
                style={styles.modalImage}
              />
              <MaterialCommunityIcons
                name="map-marker"
                color="#245BF5"
                size={20}
                style={styles.modalIconLocation}
              />
              <Text style={styles.modalLocation}>{modalLocation}</Text>
            </View>
            <View style={styles.modalBox}>
              <MaterialCommunityIcons
                name="clock-time-ten-outline"
                color="#245BF5"
                size={26}
                style={styles.modalIconTime}
              />
              <Text style={styles.modalDate}>{modalDate}</Text>
              <Text style={styles.modalTime}>
                {modalTime.substr(0, 5)} - {modalTime.substr(0, 3) + 30}
              </Text>
              <View
                style={{
                  borderBottomColor: "#E2E2E2",
                  borderBottomWidth: 2,
                }}
              />
              <MaterialCommunityIcons
                name="trophy"
                color="#245BF5"
                size={26}
                style={styles.modalIconTime}
              />
              <Text style={styles.modalDate}>Award Presence</Text>
              <Text style={styles.modalTime}>Built Different</Text>
              <View
                style={{
                  borderBottomColor: "#E2E2E2",
                  borderBottomWidth: 2,
                }}
              />
              <MaterialCommunityIcons
                name="human-male-male"
                color="#245BF5"
                size={26}
                style={styles.modalIconTime}
              />
              <Text style={styles.modalDate}>15 people are coming</Text>
              <Text style={styles.modalTime}>4 friends</Text>
            </View>
            <TouchableOpacity style={styles.modalButton}>
              <Text style={styles.modalButtonText}>I’m in!</Text>
            </TouchableOpacity>
          </>
        )}
      </RBSheet>
    </View>
  );
}

function Events({ navigation }) {
  const [eventData, setEventData] = useState(null);
  const refRBSheet = useRef();
  const [modalName, setModalName] = useState(null);
  const [modalImage, setModalImage] = useState(null);
  const [modalLocation, setModalLocation] = useState(null);
  const [modalDate, setModalDate] = useState(null);
  const [modalTime, setModalTime] = useState(null);

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
          <TouchableOpacity
            style={styles.eventButton}
            onPress={() => {
              refRBSheet.current.open();
              setModalInfo(event);
            }}
          >
            <Text style={styles.eventButtonText}>More Info</Text>
          </TouchableOpacity>
        </View>
      );
    });
  }

  const setModalInfo = (event) => {
    let name = event.location.split(",");
    let time = event.startTime.split("T");
    let location;
    console.log(event.location.length);
    location =
      event.location.length > 30
        ? event.location.substr(0, 30) + "..."
        : event.location;
    setModalName(name[2]);
    setModalImage(event.imageURL);
    setModalLocation(location);
    setModalDate(time[0]);
    setModalTime(time[1]);
  };

  return (
    <SafeAreaView>
      <ScrollView>{events}</ScrollView>
      <RBSheet
        ref={refRBSheet}
        closeOnDragDown={true}
        closeOnPressMask={true}
        height={Dimensions.get("screen").height - 100}
        customStyles={{
          wrapper: {
            backgroundColor: "transparent",
            borderRadius: 100,
          },
          draggableIcon: {
            backgroundColor: "#6D6D6D",
          },
          container: {
            borderRadius: 50,
          },
        }}
      >
        {modalName != null && (
          <>
            <Text style={styles.modalName}>{modalName}</Text>
            <View style={styles.modalImageBox}>
              <Image
                source={{
                  uri: `${modalImage}`,
                }}
                style={styles.modalImage}
              />
              <MaterialCommunityIcons
                name="map-marker"
                color="#245BF5"
                size={20}
                style={styles.modalIconLocation}
              />
              <Text style={styles.modalLocation}>{modalLocation}</Text>
            </View>
            <View style={styles.modalBox}>
              <MaterialCommunityIcons
                name="clock-time-ten-outline"
                color="#245BF5"
                size={26}
                style={styles.modalIconTime}
              />
              <Text style={styles.modalDate}>{modalDate}</Text>
              <Text style={styles.modalTime}>
                {modalTime.substr(0, 5)} - {modalTime.substr(0, 3) + 30}
              </Text>
              <View
                style={{
                  borderBottomColor: "#E2E2E2",
                  borderBottomWidth: 2,
                }}
              />
              <MaterialCommunityIcons
                name="trophy"
                color="#245BF5"
                size={26}
                style={styles.modalIconTime}
              />
              <Text style={styles.modalDate}>Award Presence</Text>
              <Text style={styles.modalTime}>Built Different</Text>
              <View
                style={{
                  borderBottomColor: "#E2E2E2",
                  borderBottomWidth: 2,
                }}
              />
              <MaterialCommunityIcons
                name="human-male-male"
                color="#245BF5"
                size={26}
                style={styles.modalIconTime}
              />
              <Text style={styles.modalDate}>15 people are coming</Text>
              <Text style={styles.modalTime}>4 friends</Text>
            </View>
            <TouchableOpacity style={styles.modalButton}>
              <Text style={styles.modalButtonText}>I’m in!</Text>
            </TouchableOpacity>
          </>
        )}
      </RBSheet>
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

  modalName: {
    fontSize: 38,
    top: 8,
    color: "#00103A",
    textAlign: "center",
  },

  modalImage: {
    width: "100%",
    height: 220,
    borderRadius: 19,
  },

  modalIconLocation: {
    top: 13,
    left: "6%",
  },

  modalLocation: {
    fontSize: 17,
    bottom: 10,
    left: "16%",
    color: "#525252",
  },

  modalIconTime: {
    top: 20,
    left: "5%",
  },

  modalDate: {
    fontSize: 17,
    bottom: 15,
    left: "18%",
    color: "#525252",
  },

  modalTime: {
    fontSize: 17,
    bottom: 15,
    left: "18%",
    color: "#828282",
  },

  modalButton: {
    height: 79,
    width: "82%",
    borderRadius: 45,
    backgroundColor: "#2491F5",
    top: 81,
    left: "9%",
    shadowColor: "#9D9D9D",
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.25,
    shadowRadius: 2.22,

    elevation: 8,
  },

  modalButtonText: {
    fontSize: 28,
    top: 16,
    left: "38%",
    color: "white",
    fontWeight: "bold",
  },

  modalBox: {
    borderWidth: 2,
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    borderBottomLeftRadius: 20,
    borderBottomRightRadius: 20,
    borderColor: "#E2E2E2",
    top: 50,
    width: "82%",
    left: "9%",
  },

  modalImageBox: {
    borderWidth: 2,
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    borderBottomLeftRadius: 20,
    borderBottomRightRadius: 20,
    borderColor: "#E2E2E2",
    width: "82%",
    left: "9%",
    top: 20,
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
