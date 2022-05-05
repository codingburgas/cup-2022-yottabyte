import React from "react";
import { StatusBar } from "react-native";
import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { Map } from "./components/Map";
import { EventSearch } from "./components/EventSearch";
import { NavigatorContext } from "./components/NavigatorContext";

<StatusBar translucent backgroundColor="transparent" />;
const Stack = createNativeStackNavigator();

function MapComponent({ navigation }) {
  return (
    <NavigatorContext.Provider value={navigation}>
      <Map />
    </NavigatorContext.Provider>
  );
}

function EventComponent({ navigation }) {
  return <EventSearch />;
}

function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator>
        <Stack.Screen
          name="MapComponent"
          component={MapComponent}
          options={{ headerShown: false }}
        />
        <Stack.Screen
          name="EventComponent"
          component={EventComponent}
          options={{ headerShown: false }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

export { App };
