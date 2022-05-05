import React, { useEffect, useState } from "react";
import { StyleSheet, View, Dimensions, Alert } from "react-native";
import { Navigation } from "./Navigation";

export function EventComponent() {
  return (
    <>
      <Navigation />
    </>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  }
});
