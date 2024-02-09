db.createUser(
    {
        user: "InnoClinic",
        pwd: "InnoClinic123$",
        roles: [
            {
                role: "readWrite",
                db: "InnoClinicServices"
            }
        ]
    }
);
// db.createCollection("test"); //MongoDB creates the database when you first store data in that database