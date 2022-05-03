import React from "react";
import { StyleSheet, View, SafeAreaView, Dimensions } from "react-native";
import MapView from 'react-native-maps';

export function Map () {
  return (
    <SafeAreaView style={styles.container}>
      <MapView
        style={styles.map}
        region={{
          latitude: 42.4991874040583,
          longitude: 27.46135711669922,
          latitudeDelta: 0.0922,
          longitudeDelta: 0.0421,
        }}
      />
    </SafeAreaView>
  );
};

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
