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
import Svg, { Path } from "react-native-svg";

export function Navigation() {
  return (
    <View style={styles.navigation}>
      <TouchableOpacity
        style={styles.mapIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Svg width={34} height={45}>
          <Path
            d="M16.96.884C7.582.884 0 8.404 0 17.702c0 12.613 16.96 26.522 16.96 26.522s16.958-13.908 16.958-26.522C33.918 8.404 26.335.884 16.96.884Zm0 22.825c-3.344 0-6.058-2.691-6.058-6.007 0-3.315 2.714-6.006 6.057-6.006 3.343 0 6.057 2.69 6.057 6.006 0 3.316-2.713 6.007-6.057 6.007Z"
            fill="#B2B1B6"
          />
        </Svg>
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.searchIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Svg width={45} height={44}>
          <Path
            d="m32.708 36.321-.47-.424c-.741-.667-1.85-.668-2.693-.137-2.88 1.812-5.91 1.979-9.94 1.979C9.154 37.739.68 29.489.68 19.312.68 9.134 9.153.884 19.606.884c10.453 0 18.925 8.25 18.925 18.428 0 3.828-.286 6.793-2.063 9.556-.593.923-.54 2.167.274 2.901l.334.3 6.461 6.498a2 2 0 0 1-.023 2.843l-1.477 1.439a2 2 0 0 1-2.809-.018l-6.52-6.51Zm0-17.01c0-7.058-5.852-12.757-13.102-12.757S6.503 12.253 6.503 19.312c0 7.059 5.853 12.757 13.103 12.757s13.102-5.698 13.102-12.757Z"
            fill="#B2B1B6"
          />
        </Svg>
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.notificationIcon}
        title="Map"
        // onPress={() => navigation.navigate("Map")}
      >
        <Svg
          width={37}
          height={48}
        >
          <Path
            d="M18.625 47.06c1.638 0 3.078.255 3.858-.404.843-.713-.278-1.837-1.383-1.837h-4.95c-1.105 0-2.226 1.124-1.383 1.837.78.66 2.22.404 3.858.404Zm17.901-12.485V20.809c0-7.277-6.957-12.634-13.085-14.773-.853-.297-1.46-1.08-1.46-1.983v-.211C21.981 1.716 20.482 0 18.625 0c-1.857 0-3.357 1.716-3.357 3.842v.213c0 .902-.606 1.684-1.457 1.982C7.698 8.173.724 13.509.724 20.809V40.258a2 2 0 0 0 2 2h31.802a2 2 0 0 0 2-2v-5.683Zm-4.475.56a2 2 0 0 1-2 2H7.199a2 2 0 0 1-2-2V20.81c0-6.352 7.854-10.565 13.426-10.565S32.05 14.457 32.05 20.81v14.327Z"
            fill="#B2B1B6"
          />
        </Svg>
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
    borderRadius: 1,
  },
});
