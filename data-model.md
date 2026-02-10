Data Model
This section describes the internal data structures of the Sardine Grasshopper parking solver. Understanding these structures is key to enabling persistent save/load of parking layouts and for developers who wish to extend or inspect the plugin's output. We present a JSON-style schema for the parking layout data, covering the main entity types: Spot, Road, Island, AccessPoint, along with relevant metadata. This format is both human-readable and suitable for serialization (e.g., one could export a layout as JSON to save it outside of Grasshopper, or use these definitions to reconstruct a saved state in Grasshopper).
Note: All linear dimensions are stored in centimeters for consistency. Angles are in degrees (0–360). Coordinates are given in some consistent coordinate system (typically the Rhino document’s world XY plane, unless otherwise noted).
Overall Structure
At a high level, the parking layout can be thought of as a JSON object with top-level keys corresponding to each component type and a metadata section. For example:
{
  "boundary": { /* omitted for brevity: could store the input boundary polyline */ },
  "spots": [ /* array of Spot objects */ ],
  "roads": [ /* array of Road objects */ ],
  "islands": [ /* array of Island objects */ ],
  "accessPoints": [ /* array of AccessPoint objects */ ],
  "metadata": { /* global metadata fields */ }
}
boundary: (Optional) The closed polyline defining the plot boundary. This could be stored as an array of point coordinates or indices referencing Rhino geometry. The solver primarily uses it as input, so it might not always be explicitly serialized in output, but including it is useful for completeness in a save file.
spots: An array of Spot objects, each representing a parking stall (regular or special).
roads: An array of Road objects, representing drive aisles or circulation roads in the lot.
islands: An array of Island objects, representing non-driveable areas (pedestrian zones, medians, buffers, or simply unusable slivers of space).
accessPoints: An array of AccessPoint objects, each marking an entrance/exit to the parking lot for vehicles (and possibly pedestrian access).
metadata: A dictionary of additional information about the layout (e.g., counts, percentages, tags, and user-defined parameters that were used to generate the solution).
Below we define each object type in detail, with their fields and intended data types. Example snippets are provided to illustrate the format.
Spot
A Spot object represents a single parking space in the layout. This includes standard spots, handicapped spots, elderly spots, and any other stall type (they share the same structure with some flags or attributes distinguishing special types).
Fields:
id (integer): Unique identifier for the spot within the layout. This is useful for indexing and referencing spots (for example, if we need to tag or remove a specific spot by ID). IDs should be unique and might be assigned sequentially.
type (string): The category of spot. Typical values: "regular", "handicap" (for accessible/disabled), "elderly" (or "senior"), "compact" (if small-car spots were supported), "motorcycle", "bicycle", etc. The plugin’s special spot feature will label those accordingly (e.g., handicap or elderly). Regular spots have type "regular". This field can also be used as a general tag for custom spot types (for example, one could mark "EV" for an electric vehicle charging spot, though by default the solver doesn’t do this automatically).
position (object): The location and orientation of the spot. This can be represented in multiple ways:
center (point coordinate, [x, y]): The center point of the parking stall (on the ground plane).
orientation (number, degrees): The angle of the spot relative to some reference axis (e.g., 0° might align with the x-axis of the lot’s coordinate system). For perpendicular parking aligned with the lot, you might see 0° or 90°; for angled parking, common values are 45°, 60° etc. Orientation defines how the spot’s length axis is rotated.
Optionally, instead of center+orientation, one might store the polygon corners of the stall:
corners: an array of 4 points (each [x,y]) representing the parking space rectangle vertices in order. This explicitly gives the spot’s footprint polygon. This may be more verbose but is unambiguous for reconstruction.
dimensions (object): The size of the parking spot.
width (number): The width of the spot (usually measured perpendicular to the car’s forward direction), in cm. For a 90° regular spot this is the stall width (e.g. 250). For angled spots, this still refers to the width of the stall (bumper-to-bumper clearance in the direction parallel to the aisle).
length (number): The length of the spot (typically the distance from aisle to the front of the stall), in cm. Standard is ~500 for perpendicular. For parallel parking, length would be much larger (~600–670).
These values reflect the effective design dimensions and should correspond to the values set in the solver’s spot settings.
adjacentRoadId (integer, optional): The id of the Road object (from the roads list) that this spot directly connects to. Essentially, which drive aisle serves this parking stall. For example, interior perpendicular spots would reference the central aisle’s ID; outer boundary spots might reference a perimeter road’s ID or a generic boundary id. This field helps in understanding the parking layout connectivity (which spots are on which aisle).
specialAttributes (object, optional): Additional fields present if the spot is of a special type.
For handicap spots, this might include:
accessLaneWidth (number, cm): The extra width of the adjacent access aisle or cross-hatched area reserved for wheelchair maneuvering. E.g., 120 cm. In the plugin, when a spot is morphed to “Handicap”, an additional lane width is specified. This field captures that value. It essentially means the spot occupies additional lateral space or has a clear zone of that width on one side.
markings (boolean or string): Could indicate if special pavement markings are applied (wheelchair symbol, etc.). The solver doesn’t output graphics, but a true/false or enum here could mark that this spot needs special painting/signage.
For elderly spots, typically they might not require extra physical dimensions, but we could include an attribute like reservedFor = "elderly" or simply rely on type = "elderly".
For other types, e.g. "bicycle" or "motorcycle", one might include number of racks or motorcycle count that fit, etc. (See Bicycle Spots below for how bikes are handled via islands or converted spots.)
tag (string, optional): A user-defined tag or label for the spot. This is a free-form field that can store things like spot numbering (“A1”, “A2”, etc.), or a classification like “visitor”, “staff”, “electric”, etc. It’s distinct from type in that type is meant for broad categories affecting geometry, whereas tag could be any custom label not impacting geometry. For example, you might tag certain regular spots as “compact-only” if your design intends some spots for small cars (though the solver doesn’t explicitly have a compact mode, a user could still decide that some end spots be designated compact).
Example:
{
  "id": 17,
  "type": "handicap",
  "position": { "center": [1200.0, 350.0], "orientation": 90 },
  "dimensions": { "width": 270, "length": 500 },
  "adjacentRoadId": 5,
  "specialAttributes": {
    "accessLaneWidth": 120,
    "markings": true
  },
  "tag": "ADA-1"
}
This describes a handicapped spot (ID 17) oriented at 90°, centered at (1200, 350). It’s slightly wider (270 cm width) and has an extra 120 cm access lane. It’s connected to road ID 5. The tag "ADA-1" could indicate it’s the first accessible spot.
Regular spots would be similar but without specialAttributes. For instance:
{
  "id": 18,
  "type": "regular",
  "position": { "center": [1200.0, 460.0], "orientation": 90 },
  "dimensions": { "width": 250, "length": 500 },
  "adjacentRoadId": 5
}
This is a standard 2.5 m by 5 m spot, aligned with the same road.
Road
A Road object represents a driving aisle or internal road segment within the parking lot. These could be the main access drive, axial divider roads, or peripheral circulation lanes. The solver might generate multiple road segments (especially if an axial road is used to split the lot).
Fields:
id (integer): Unique identifier for the road.
type (string): Category of road. Possible values:
"access" – for primary entry/exit roads. Typically these connect an AccessPoint into the lot. They often run perpendicular to the street and then along the lot’s length. They might have different width settings (see width).
"axial" – for internal divider roads that run across the lot, splitting parking sections. These are often the ones input or automatically placed to break a large area.
"perimeter" – for any circulation path running around the outer edge of the parking (if the design includes a ring road or outer drive lane).
"aisle" – a generic term for any parking aisle (two-way or one-way) that has parking spots on one or both sides. In many cases, "access" roads also double as aisles with spots, but we differentiate the main entrance drive as access if needed. The plugin doesn’t explicitly label types in output, but for documentation it’s useful to think in these terms.
width (number): The driving width of this road, in cm. For two-way aisles or main roads, this could be ~600 cm. For one-way angled aisles, maybe ~350 cm. The plugin distinguishes Access Width vs Axial Width in settings, so roads inheriting those would carry those widths. For example, a road of type "access" might use the Access Width parameter, whereas type "axial" uses the Axial Width parameter. Storing the width per road allows correct reconstruction of geometry.
geometry (object): The physical path or surface of the road. There are a couple of ways to represent it:
centerLine: an array of points [[x,y], [x,y], ...] forming the polyline center of the road. This is useful if the road is not a straight line (e.g., a curved or bent path through the lot or an L-shaped aisle). The centerLine points should lie roughly mid-lane.
endPoints: a simpler variant for a straight road segment: two points [start, end] indicating the centerline endpoints.
polyline: one could store the entire polyline outline of the road surface (though that can be derived from centerLine and width).
surfaceCorners: If needed, the four corner points of the road surface polygon (for a straight segment). Typically, however, roads might intersect or form T-junctions, so it’s easier to keep them as lines plus width.
The solver likely outputs road geometry as curves in Rhino. For data, storing the centerline plus width is sufficient to recreate an aisle as a region.
connectedSpotIds (array of integers, optional): A list of ids of Spot objects that are accessed directly by this road. Essentially, all spots on either side of the aisle. This can be derived by looking at each spot’s adjacentRoadId, but having it here is convenient for quickly grouping spots by aisle.
connectedAccessPointIds (array of integers, optional): If this road ties into one or more AccessPoints (entrances/exits), list them here. For instance, the main driveway road might connect to AccessPoint A (entrance from street).
oneWay (boolean, optional): Indicates if traffic on this road is one-way. By default, internal aisles might be assumed two-way, but if an angled parking configuration is used, certain aisles could be one-way (with angled spots oriented for that flow). Marking oneWay = true is important for any signage or circulation logic outside the solver’s scope. (The plugin’s data doesn’t explicitly say this, but the user’s chosen parking angle can imply it. We include the field to allow storing that information if needed.)
Example:
{
  "id": 5,
  "type": "access",
  "width": 600,
  "centerLine": [[0.0, 500.0], [1200.0, 500.0]],
  "connectedSpotIds": [17, 18, 19, 20, 21, 22],
  "connectedAccessPointIds": [1],
  "oneWay": false
}
This describes a straight two-way access aisle (6 m wide) from point (0,500) to (1200,500). It has spots with IDs 17–22 along it, and is connected to an AccessPoint with id 1 (perhaps the main entrance). It’s marked two-way.
Another example for an axial road:
{
  "id": 6,
  "type": "axial",
  "width": 500,
  "centerLine": [[600.0, 200.0], [600.0, 800.0]],
  "connectedSpotIds": [30, 31, 32, 33],
  "connectedAccessPointIds": [],
  "oneWay": false
}
This could represent a cross road (5 m wide) that runs vertically at x=600 through the lot from y=200 to y=800. It has some spots along it (IDs 30–33 perhaps on its side). No direct external access.
If there were angled one-way aisles, we could see oneWay: true on those, with width per design.
Island
An Island object represents a piece of land in the lot that is not driveable roadway and not a parking stall – essentially a leftover or intentionally created area for other purposes. Islands often serve as medians, pedestrian walkways, planters, or simply buffer spaces for turning.
Fields:
id (integer): Unique identifier for the island.
geometry (object): The shape of the island. This could be:
boundary: an array of points [[x,y], ...] forming a closed polyline of the island’s perimeter.
If the island is very simple (like rectangular or circular), one could store center and dimensions, but given arbitrary shapes, a polyline is more general.
If corners are filleted (rounded), the polyline might approximate them or additional data might note fillet radius.
area (number, optional): The area of the island in square cm (for reference or filtering out tiny islands).
type (string, optional): Descriptive category of island. Possible values:
"pedestrian" – if the island is actually a pedestrian sidewalk or refuge. For example, if Pedestrian Settings style is set to sidewalks, the solver might produce a sidewalk island along edges or between rows; those could be flagged as pedestrian islands.
"landscape" – if intended as a landscape planter or green area (often islands at row ends are landscaping).
"buffer" – a generic buffer space not designated for anything specific (just leftover).
"bike_pad" – if this island has been repurposed to contain bicycle parking (see below).
The plugin doesn’t automatically assign these labels, but a post-process or the user could tag islands for intended use. We include the field for completeness.
adjacentRoadIds (array of integers, optional): List of road IDs that border this island (if any). For example, a center pedestrian median between two parking aisles might have road IDs on either side.
adjacentSpotIds (array of integers, optional): List of spot IDs that border the island. E.g., an island at the end of a row might have two spots adjacent (one on each side of the island).
containsBikeSpots (boolean or object, optional): This field is used if the island has been allocated for bicycle parking. If true or an object, it indicates this island is used to host bike racks.
If an object, it can provide details such as number of bike racks or bikes accommodated, etc. For example:
bikeCapacity: number of bicycles that can be parked in this island area.
bikeLayout: perhaps an arrangement description (like parallel racks vs angled racks).
The plugin’s Bike Picker component allows selecting islands to convert to bike spots. In data terms, those islands could then carry info that they are serving bikes instead of being empty. Also, some car spots might be converted to bike parking (see next section).
Example:
{
  "id": 12,
  "geometry": {
    "boundary": [[1150.0, 320.0], [1200.0, 320.0], [1200.0, 480.0], [1150.0, 480.0]]
  },
  "area": 7500,
  "type": "pedestrian",
  "adjacentRoadIds": [5],
  "adjacentSpotIds": [17, 18],
  "containsBikeSpots": false
}
This could be a rectangular island (perhaps a sidewalk) between spots 17 & 18 and road 5. It’s marked pedestrian, not used for bikes, about 0.75 m² (which seems small – just an example).
Another example, an island used for bikes:
{
  "id": 15,
  "geometry": {
    "boundary": [[600.0, 500.0], [630.0, 500.0], [630.0, 530.0], [600.0, 530.0]]
  },
  "area": 900,
  "type": "bike_pad",
  "adjacentRoadIds": [],
  "adjacentSpotIds": [],
  "containsBikeSpots": {
    "bikeCapacity": 6,
    "bikeLayout": "two-tier rack"
  }
}
This shows an island (30 cm by 30 cm – unrealistic, but just to illustrate fields) designated as a bike pad with capacity for 6 bicycles. In practice, a bike pad might be larger (e.g. repurposing a whole car spot or an end island of size perhaps 250 × 500 cm could hold ~10 bikes). The data model allows us to capture that information.
AccessPoint
An AccessPoint represents an entry or exit point on the boundary of the parking lot where vehicles (or pedestrians) can come in or out. In the solver, these are input as points on the boundary curve, and typically at least one is required to position the internal roads.
Fields:
id (integer or string): Identifier for the access point. Could be numeric or a label like "A", "B" if there are few. We use numeric in examples for consistency.
location (point coordinate [x, y]): The exact position of the access on the boundary polyline. This should lie on or very near the boundary curve coordinates.
width (number, optional): The width of the access opening, in cm. For example, if the entrance is a curb cut 1 car wide, maybe ~350 cm; if it’s a 2-lane entrance/exit, maybe ~600 cm or more. The plugin’s Access Width parameter might dictate this for drawing the curb return geometry. Storing it ensures that when reconstructing, we know how wide a gap in the boundary or how wide the connecting road should be.
orientation (number, optional): The direction from the access point into the lot, in degrees. This can be inferred from the boundary segment’s orientation at that point. It’s useful if we need to align the connecting road. For instance, an AccessPoint on the south edge of a rectangular lot might have orientation ~90° (pointing north into the lot). If not stored, it can be computed by projecting inward perpendicular to the boundary.
connectionRoadId (integer, optional): The ID of the Road object that connects to this access. Typically, the solver will create a short road segment or align an aisle to meet the entrance. This field links the access to the internal road network. For example, road ID 5 (in the earlier Roads example) had connectedAccessPointIds [1], so this AccessPoint 1 would have connectionRoadId 5.
type (string, optional): The kind of access – e.g., "entry_only", "exit_only", or "entry_exit" for dual use. The solver doesn’t differentiate this (assumes a general purpose entry/exit), but in data one could specify it if needed for signage or flow modeling.
pedestrianAccess (boolean, optional): If true, this point is also used as a pedestrian entrance/exit (e.g., a walkway to a building or sidewalk is co-located here). The Pedestrian Settings allow specifying pedestrian access points. In many cases, the pedestrian access might coincide with a vehicular access or be separate. If separate, we might represent purely pedestrian gates similarly, but possibly better as a distinct type (for now, we can include a flag or simply include pedestrian-only points as AccessPoints with a type field like "pedestrian").
Example:
{
  "id": 1,
  "location": [0.0, 500.0],
  "width": 500,
  "orientation": 0,
  "connectionRoadId": 5,
  "type": "entry_exit",
  "pedestrianAccess": true
}
This indicates an access centered at (0,500) on the boundary (likely the middle of the west edge of a lot), 5 m wide opening. Orientation 0° could mean it points eastward into the lot (assuming 0° is along the positive X direction, which in this scenario goes into the lot). It’s used for both entry and exit. It connects to road ID 5 (so road 5 starts at this point). Also marked as a pedestrian access – maybe there’s a sidewalk there that people also use to walk in.
If there were a separate pedestrian gate on the north side, for example, we might have:
{
  "id": 2,
  "location": [600.0, 1000.0],
  "width": 150,
  "orientation": 270,
  "connectionRoadId": null,
  "type": "pedestrian",
  "pedestrianAccess": true
}
This would be a 1.5 m wide pedestrian entrance (no road, just a footpath) at the middle of the north boundary (assuming 1000 is top Y), facing outward (270° pointing south into the lot). No road connection since cars don’t go here.
Metadata
The metadata section aggregates global information and parameters used in the solution. This can include counts, percentages, and high-level settings that influenced the solver. Storing these can be very useful for reconstruction or analysis.
Key metadata fields might be:
totalSpots (integer): Total number of parking spots generated.
regularCount, handicapCount, elderlyCount, otherCount (integers): Breakdown of spot counts by type. For example, handicapCount = 2, elderlyCount = 1, regularCount = 47 (in a lot of 50 total).
handicapPercentage (number): The percentage of spots intended to be accessible (as input by the user). E.g., 4 (%). This along with totalSpots can compute expected count. Storing the actual percentage helps trace if the user overrode it or if rounding occurred.
elderlyPercentage (number): Similar for elderly (or other special categories if present).
bicyclePercentage (number): If the user specified that X% of total should be converted to bike parking (via the Bike Picker’s Amount setting). This might not correspond one-to-one with a count of spots, since one car spot might turn into multiple bike slots. But it’s an input reference.
angle (number): The parking angle used for all stalls (e.g., 90 for perpendicular, 45 for angled, etc.). This is essentially duplicative of what each spot’s orientation might show, but it’s good to record the overall pattern. If outer and inner spots had different angles (not typical in current version, but if it were allowed), this could perhaps be an array or separate outerAngle/innerAngle.
aisleWidth (number): The width used for main aisles (if uniform). If different roads have different widths (access vs axial), one might store both, e.g. accessRoadWidth and axialRoadWidth. For example, accessRoadWidth = 600, axialRoadWidth = 500, if those were the settings.
skirtOffset (number): The boundary offset used (if any) as a “skirt” – e.g., the inner tolerance from boundary before placing parking. The default is 50 cm (0.50 m) which ensures a little buffer from the lot edge. If the user changed it, storing it is wise.
perimeterMode (integer): The selected perimetral layout mode (0, 1, 2, or 3) that was used. This affects how outer and inner spots were distributed:
0 = only inner spots (no boundary parking),
1 = boundary + inner,
2 = boundary on both sides + inner,
3 = like 2 but with inner roads having single access (dead-end aisles).
Recording this explains the presence/absence of certain spots.
referenceEdgeIndex (integer): If an edge of the boundary was chosen as a reference for layout orientation, this could note which one (e.g., index 2 of the polyline’s segments).
snapAccess (boolean): Whether the solver snapped the internal layout to align with the access point for better distribution. True/false.
isMetric (boolean): Always true in our context (since we use cm). If in some use someone ran it in imperial units, this could flag unit context, but for consistency we assume metric.
generationTimestamp (string or number): Timestamp or tick when the solution was generated, if needed for versioning.
solverVersion (string): Version of the plugin or algorithm used. E.g., "1.0.0". Could help if formats evolve.
tags (object): A place for any additional user-defined metadata or labels that apply to the whole layout. This could include:
Project or site identifiers,
A scenario name (if comparing multiple parking layout scenarios),
Notes or flags (like "manualAdjustmentsMade": true if the user tweaked after generation, etc.). Essentially, this is an extensible dictionary.
Example metadata:
{
  "totalSpots": 50,
  "regularCount": 45,
  "handicapCount": 3,
  "elderlyCount": 2,
  "handicapPercentage": 5.0,
  "elderlyPercentage": 4.0,
  "bicyclePercentage": 10.0,
  "angle": 90,
  "accessRoadWidth": 600,
  "axialRoadWidth": 500,
  "skirtOffset": 50,
  "perimeterMode": 1,
  "referenceEdgeIndex": 0,
  "snapAccess": true,
  "isMetric": true,
  "generationTimestamp": "2026-02-10T18:56:00Z",
  "solverVersion": "1.0.0",
  "tags": {
    "project": "CentralMall Parking Scheme A",
    "note": "Adjusted for 5% accessible per BG code."
  }
}
This metadata tells us: 50 spots total, with 3 accessible (which is 6% actually, maybe slightly above 5% setting to meet minimum 3), 2 elderly, the design used perpendicular parking (90°), main aisles 6 m, cross aisles 5 m, 50 cm boundary offset, boundary+inner mode, aligned to edge 0, snapped to entrance, etc. The tags give a custom project name and a note.
Bicycle Parking Considerations
(This is a special case bridging Spots and Islands, worth detailing.)
Bicycle parking in the solver is handled by either converting some normal car spots to bike racks or by using islands to host bikes. The data model can capture bike parking in two ways:
Converted Spots: If a regular spot is repurposed for bikes, the Spot’s type might be set to "bicycle" (or "bike_converted"). Its dimensions would remain those of a car space (since physically it’s the same area, just used differently). We might then interpret that such a spot can accommodate multiple bikes. Additional info like bikeCapacity could be added under Spot’s specialAttributes. However, the plugin likely doesn’t do partial usage of a spot automatically; it would remove that spot from car count and one would manually place bike racks.
Islands as Bike Pads: More commonly, the plugin’s Bike Picker works by selecting an island or creating a small island to serve as bike parking. In data terms, that island would have type: "bike_pad" and containsBikeSpots details as shown above. We might not list individual bike racks as “spots” since they aren’t car spots, unless we wanted to list each bike stand as a tiny spot (which is usually overkill). Instead, we treat the whole island as one parking facility for bikes, and the bikeCapacity in the island’s data indicates how many bicycles it holds.
In summary, the data model is flexible enough to record all components of the parking solution. By saving the spots, roads, islands, accessPoints, and metadata as described, one can fully reconstruct the parking layout. This persistent representation enables features like undo/redo of layout changes, transferring solutions between files, or even feeding the data into other tools (for simulation or documentation).
All objects can be serialized to JSON (as illustrated) or XML or any preferred format. JSON is natural for its readability and direct mapping to the structures described.
Persistence note: Currently, the plugin itself does not automatically save all this data (especially the special spot state issue where handicap flags reset on reopen). By implementing a serialization of the above structure, one could preserve the complete state. For example, a Grasshopper definition could output this JSON to a text file when saving, and reload it to restore state. Future versions of the plugin might integrate such persistence.
For developers, these definitions should make it clear how each parking element is structured internally, facilitating extension (like adding new spot types or integrating with analysis tools). By adhering to this data model, any custom scripts or components can interact with Sardine’s output in a consistent way – whether to count elements, filter them (e.g., find all handicap spots), or transform the layout.