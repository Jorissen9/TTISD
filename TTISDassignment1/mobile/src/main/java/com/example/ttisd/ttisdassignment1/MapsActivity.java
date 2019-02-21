package com.example.ttisd.ttisdassignment1;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListAdapter;
import android.widget.PopupWindow;
import android.widget.RelativeLayout;
import android.widget.Toast;
import android.app.PendingIntent;
import android.content.Intent;
import android.util.Log;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.GoogleMap.OnMarkerClickListener;
import com.google.android.gms.maps.GoogleMapOptions;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;

import com.google.android.gms.location.Geofence;
import com.google.android.gms.location.GeofencingClient;
import com.google.android.gms.location.GeofencingRequest;
import com.google.android.gms.location.LocationServices;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;

import java.util.ArrayList;


public class MapsActivity extends FragmentActivity implements OnMarkerClickListener, OnMapReadyCallback {
    private static final String TAG = "MapsActivity";

    private GoogleMap mMap;
    private Context mContext;
    private PopupWindow mPopupWindow;
    private Fragment mMapLayout;

    private EditText titleText;
    private EditText todoListText;
    private Marker currentMarker;

    private static final float GEO_RADIUS = 50.0f;
    private static final long GEO_EXPIRATION_IN_MILLISECONDS = Long.MAX_VALUE;

    private GeofencingClient geofencingClient;
    private ArrayList<Geofence> geofenceList;
    private PendingIntent geofencePendingIntent;


    private static final LatLng PXL = new LatLng(50.9382073, 5.34806385);
    private static final LatLng UHASSELT = new LatLng(50.9262009, 5.39275314);
    private static final LatLng CORDA = new LatLng(50.9526418, 5.3500728);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        mContext = getApplicationContext();

        setContentView(R.layout.activity_maps);

        geofencingClient = LocationServices.getGeofencingClient(this);
        geofenceList = new ArrayList<>();
        geofencePendingIntent = null;

        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);
    }

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    @Override
    @SuppressWarnings("MissingPermission")
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap;

        FragmentManager fragmentManager = this.getSupportFragmentManager();
        mMapLayout = fragmentManager.findFragmentById(R.id.map);

        // Add some markers to the map, and add a data object to each marker.
        addMapMarker(PXL, "Pxl", "Niets meer");
        addMapMarker(UHASSELT, "UHasselt", "Dit jaar slagen\nDiploma halen");
        addMapMarker(CORDA, "Corda", "Start-up promoten");

        // Set current location to the camera
        mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(UHASSELT, 12.0f));

        if (!checkPermissions()) {
            // No permission granted
            return;
        }

        mMap.setMyLocationEnabled(true);

        // Set a listener for marker click.
        mMap.setOnMarkerClickListener(this);
        mMap.setOnMapClickListener(new GoogleMap.OnMapClickListener() {

            @Override
            public void onMapClick(LatLng latLng) {
                Marker newMarker = addMapMarker(latLng);
                createMessagebox(newMarker);

                // Animating to the touched position
                mMap.animateCamera(CameraUpdateFactory.newLatLng(latLng));
            }
        });
    }

    @Override
    public boolean onMarkerClick(Marker marker) {
        createMessagebox(marker);


        // Return false to indicate that we have not consumed the event and that we wish
        // for the default behavior to occur (which is for the camera to move such that the
        // marker is centered and for the marker's info window to open, if it has one).
        return false;
    }

    private boolean checkPermissions() {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION)
                != PackageManager.PERMISSION_GRANTED
                && ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION)
                != PackageManager.PERMISSION_GRANTED)
        {
            ActivityCompat.requestPermissions(this, new String[]{
                    Manifest.permission.ACCESS_COARSE_LOCATION,
                    Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        }
        return true;
    }

    private Marker addMapMarker(LatLng latLng) {
        return addMapMarker(latLng, "", "");
    }

    @SuppressWarnings("MissingPermission")
    private Marker addMapMarker(LatLng latLng, String title, String descr) {
        Marker newMarker = mMap.addMarker(new MarkerOptions()
                .position(latLng)
                .title(title)
                .snippet(descr));

        geofenceList.add(new Geofence.Builder()
                // Set the request ID of the geofence. This is a string to identify this geofence.
                .setRequestId(title + " at " + latLng.toString())
                .setCircularRegion(latLng.latitude, latLng.longitude, GEO_RADIUS)
                .setExpirationDuration(GEO_EXPIRATION_IN_MILLISECONDS)
                .setTransitionTypes(Geofence.GEOFENCE_TRANSITION_ENTER | Geofence.GEOFENCE_TRANSITION_EXIT)
                .build());

        GeofencingRequest request = new GeofencingRequest.Builder()
                                    .setInitialTrigger(GeofencingRequest.INITIAL_TRIGGER_ENTER)
                                    .addGeofences(geofenceList)
                                    .build();

        if (checkPermissions()) {
            geofencingClient.addGeofences(request, getGeofencePendingIntent());
            Log.i(TAG, "Added geofences " + request.toString());
        } else {
            Log.e(TAG, "No permission to add geofence");
        }

        return newMarker;
    }

    private void createMessagebox(Marker marker) {
        currentMarker = marker;
        // Initialize a new instance of LayoutInflater service
        LayoutInflater inflater = (LayoutInflater) mContext.getSystemService(LAYOUT_INFLATER_SERVICE);

        // Inflate the custom layout/view
        View customView = inflater.inflate(R.layout.todo_list_popup,null);

                /*
                    Parameters
                        contentView : the popup's content
                        width : the popup's width
                        height : the popup's height
                */
        // Initialize a new instance of popup window
        if(mPopupWindow != null) {
            mPopupWindow.dismiss();
        }
        mPopupWindow = new PopupWindow(
                customView,
                RelativeLayout.LayoutParams.WRAP_CONTENT,
                RelativeLayout.LayoutParams.WRAP_CONTENT
        );
        mPopupWindow.setFocusable(true);

        // Set an elevation value for popup window | Call requires API level 21
        if(Build.VERSION.SDK_INT>=21){
            mPopupWindow.setElevation(5.0f);
        }

        // Get a reference for the custom view close button
        titleText = customView.findViewById(R.id.title);
        todoListText = customView.findViewById(R.id.todo);
        titleText.setText(currentMarker.getTitle());
        todoListText.setText(currentMarker.getSnippet());

        Button updateButton = customView.findViewById(R.id.button_update);
        Button closeButton = customView.findViewById(R.id.button_cancel);
        Button removeButton = customView.findViewById(R.id.button_remove);

        // Set a click listener for the popup window close button
        updateButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // Dismiss the popup window
                System.out.println(titleText.getText());
                System.out.println(todoListText.getText());
                currentMarker.setTitle(String.valueOf(titleText.getText()));
                currentMarker.setSnippet(String.valueOf(todoListText.getText()));
                mPopupWindow.dismiss();
            }
        });

        // Set a click listener for the popup window close button
        closeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // Dismiss the popup window
                mPopupWindow.dismiss();
            }
        });

        // Set a click listener for the popup window close button
        removeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                currentMarker.remove();

                // Delete geofence monitor
                geofencingClient.removeGeofences(getGeofencePendingIntent());

                // Dismiss the popup window
                mPopupWindow.dismiss();
            }
        });

                /*
                    Parameters
                        parent : a parent view to get the getWindowToken() token from
                        gravity : the gravity which controls the placement of the popup window
                        x : the popup's x location offset
                        y : the popup's y location offset
                */
        // Finally, show the popup window at the center location of root relative layout
        mPopupWindow.showAtLocation(mMapLayout.getView(), Gravity.CENTER,0,0);
    }

    private PendingIntent getGeofencePendingIntent() {
        if (geofencePendingIntent != null) {
            return geofencePendingIntent;
        }

        Intent intent = new Intent(this, GeofenceBroadcastReceiver.class);

        geofencePendingIntent = PendingIntent.getBroadcast(
                this, 0,
                intent, PendingIntent.FLAG_UPDATE_CURRENT);

        return geofencePendingIntent;
    }
}
