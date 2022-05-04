import React, { useEffect, useState } from "react";
import { StyleSheet, View, Dimensions, Pressable } from "react-native";
import { NavigationContainer } from '@react-navigation/native';
// import { createStackNavigator } from "@react-navigation/stack";

export function Navigation() { 
  console.log(Dimensions.get("window").height);
  return (
    <View style={styles.navigation}></View>
  );
}

const styles = StyleSheet.create({
  navigation: {
    width: Dimensions.get("window").width,
    height: Dimensions.get("screen").height -
    (Dimensions.get("screen").height - Dimensions.get("window").height) -
    802,
  },
});
