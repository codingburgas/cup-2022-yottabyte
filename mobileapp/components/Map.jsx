import React, { useEffect, useState } from "react";
import { StyleSheet, View, SafeAreaView, Dimensions, Alert } from "react-native";
import MapView from "react-native-maps";
import * as Location from "expo-location";

export function Map() {
  const [location, setLocation] = useState(null);

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

      while (true) {
        const location = await Location.getLastKnownPositionAsync({
          accuracy: 6,
          distanceInterval: 1,
          timeInterval: 1000,
        });
        setLocation(location);
      }
    })();
  }, []);

  return (
    <SafeAreaView style={styles.container}>
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
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
    alignItems: "center",
    justifyContent: "center",
  },

  map: {
    width: Dimensions.get("window").width,
    height: Dimensions.get("window").height,
  },
});
