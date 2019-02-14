package com.example.ttisd.ttisdassignment1;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.FragmentActivity;
import android.widget.Toast;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.GoogleMap.OnMarkerClickListener;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;

public class MapsActivity extends FragmentActivity implements OnMarkerClickListener, OnMapReadyCallback {

    private GoogleMap mMap;

    private static final LatLng PXL = new LatLng(50.9382073, 5.34806385);
    private static final LatLng UHASSELT = new LatLng(50.9262009, 5.39275314);
    private static final LatLng CORDA = new LatLng(50.9526418, 5.3500728);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_maps);
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
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap;

        // Add some markers to the map, and add a data object to each marker.
        Marker mPxl = mMap.addMarker(new MarkerOptions()
                .position(PXL)
                .title("Pxl")
                .snippet("Niets meer"));
        mPxl.setTag(0);

        Marker mUhasselt = mMap.addMarker(new MarkerOptions()
                .position(UHASSELT)
                .title("UHasselt")
                .snippet("Dit jaar slagen\nDiploma halen"));
        mUhasselt.setTag(0);

        Marker mCorda = mMap.addMarker(new MarkerOptions()
                .position(CORDA)
                .title("Corda")
                .snippet("Start-up promoten"));
        mCorda.setTag(0);

        // Set current location to the camera
        mMap.moveCamera(CameraUpdateFactory.newLatLng(UHASSELT));
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            

            return;
        }
        mMap.setMyLocationEnabled(true);

        // Set a listener for marker click.
        mMap.setOnMapClickListener(new GoogleMap.OnMapClickListener() {

            @Override
            public void onMapClick(LatLng latLng) {

                // Creating a marker
                MarkerOptions markerOptions = new MarkerOptions();

                // Setting the position for the marker
                markerOptions.position(latLng);

                // Setting the title for the marker.
                // This will be displayed on taping the marker
                markerOptions.title("Enter title");
                markerOptions.snippet("TODO list");

                // Clears the previously touched position
                mMap.clear();

                // Animating to the touched position
                mMap.animateCamera(CameraUpdateFactory.newLatLng(latLng));

                // Placing a marker on the touched position
                mMap.addMarker(markerOptions);
            }
        });
    }

    @Override
    public boolean onMarkerClick(final Marker marker) {
        Toast.makeText(this,
                "TODO:\n" + marker.getSnippet(),
                Toast.LENGTH_SHORT).show();

        // Return false to indicate that we have not consumed the event and that we wish
        // for the default behavior to occur (which is for the camera to move such that the
        // marker is centered and for the marker's info window to open, if it has one).
        return false;
    }
}
