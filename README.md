# Questonaut :rocket:

| Project | Status | Description |
|---------|--------|-------------|
| Xamarin.iOS        | ![Xamarin.iOS.BuildBadge] | The iOS version of the Questonaut App
| Xamarin.Android        | ![Xamarin.Android.BuildBadge] | The Android version of the Questonaut App

[Xamarin.iOS.BuildBadge]: https://build.appcenter.ms/v0.1/apps/1fd4e12a-47dc-42f5-8e4c-0b38beefa189/branches/dev/badge
[Xamarin.Android.BuildBadge]: https://build.appcenter.ms/v0.1/apps/6b7dc797-6c6c-4476-afcd-0afb4e9e31c5/branches/dev/badge

## Introduction

Questonaut is a multiplatform app to create and execute custom esm studies.
This Xamarin.iOS and Xamarin.Android Client are used to create a account and join these studies. Studies can be created with a external client. 

### ESM Studies

A ESM (expeirence mobile sampling) stduy collects data in relation to the current context of the user. This context could be the following for example:
  - location based
  - motion based
  - battery based
  - other mobile sensor value based
  
This context helps to better understand the data and to perform a more detailed analysis.

### Architecture & Ecosystem

Beside Xamarin the following architecure is used:
  - Firebase Auth for authenticate the users
  - Firebase Store to store the user and study data
  - Firebase Cloud Storage to store files
  - AppCenter to build and monitor the app
  
To configure the app on your machine with your API secrets and AppCenter keys see documentation.

## Information

The app is a part of my bachelor thesis at the Technical University of Vienna in cooperation with the inso institute.
