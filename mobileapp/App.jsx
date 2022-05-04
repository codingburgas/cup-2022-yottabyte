import React from "react";
import { StatusBar } from "react-native";
import { Map } from "./components/Map";
import {Navigation} from "./components/Navigation"

<StatusBar translucent backgroundColor="transparent" />;

function App() {
  return (
    <>
      <Map/>
      <Navigation/>
    </>
  );
}
export default App;
