import React, { useEffect, useState } from "react";
import {
  StyleSheet,
  View,
  Dimensions,
  TouchableOpacity,
  Image,
} from "react-native";
import { NavigationContainer } from "@react-navigation/native";
import { createStackNavigator } from "@react-navigation/stack";

export function Navigation() {
  console.log(Dimensions.get("window").height);
  return (
    <View style={styles.navigation}>
      <TouchableOpacity
        style={styles.mapIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Image
          source={require("../assets/images/mapIcon.png")}
          style={{
            width: 34,
            height: 44,
          }}
        />
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.searchIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Image
          source={require("../assets/images/searchIcon.png")}
          style={{
            width: 44,
            height: 45,
          }}
        />
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.notificationIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Image
          source={require("../assets/images/notificationIcon.png")}
          style={{
            width: 36,
            height: 48,
          }}
        />
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.accountIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Image
          source={require("../assets/images/miho.png")}
          style={{
            width: 47,
            height: 47,
          }}
        />
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  navigation: {
    width: Dimensions.get("window").width,
    height:
      Dimensions.get("screen").height -
      (Dimensions.get("screen").height - Dimensions.get("window").height) -
      802,
  },

  mapIcon: {
    position: "absolute",
    left: "12.85%",
  },

  searchIcon: {
    position: "absolute",
    left: "33.1%",
  },

  notificationIcon: {
    position: "absolute",
    left: "55.78%",
  },

  accountIcon: {
    position: "absolute",
    left: "78.46%",
    borderRadius: 1
  }
});
