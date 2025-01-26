/**
 * A class to do 3D rotations of a point about a line. See
 * <a
 * href="https://sites.google.com/site/glennmurray/Home/rotation-matrices-and-formulas">
 * Rotation Matrices and Formulas</a>
 *
 * @author Glenn Murray
 */
class RotationMatrix {
	
    // Static initialization---------------------------------------------
    /** For debugging. */
    static LOG = false;

    /** How close a double must be to a double to be "equal". */
    static TOLERANCE = 1E-9;


    // Instance variables------------------------------------------------

    /** The rotation matrix.  This is a 4x4 matrix. */
    matrix;

    /** The 1,1 entry in the matrix. */
    m11;
    m12; 
    m13; 
    m14; 
    m21; 
    m22; 
    m23; 
    m24; 
    m31;
    m32;
    m33;
    m34;


    // Constructors------------------------------------------------------
    /**
     * Build a rotation matrix for rotations about the line through (a, b, c)
     * parallel to &lt u, v, w &gt by the angle theta.
     *
     * @param a x-coordinate of a point on the line of rotation.
     * @param b y-coordinate of a point on the line of rotation.
     * @param c z-coordinate of a point on the line of rotation.
     * @param uUn x-coordinate of the line's direction vector (unnormalized).
     * @param vUn y-coordinate of the line's direction vector (unnormalized).
     * @param wUn z-coordinate of the line's direction vector (unnormalized).
     * @param theta The angle of rotation, in radians.
     */
    constructor(a, b, c, uUn, vUn, wUn, theta) {
        let l;
        if ( (l = RotationMatrix.longEnough(uUn, vUn, wUn)) < 0) {
            console.log("RotationMatrix: direction vector too short!");
            return;             // Don't bother.
        }

        // In this instance we normalize the direction vector.
        let u = uUn/l;
        let v = vUn/l;
        let w = wUn/l;

        // Set some intermediate values.
        let u2 = u*u;
        let v2 = v*v;
        let w2 = w*w;
        let cosT = Math.cos(theta);
        let oneMinusCosT = 1-cosT;
        let sinT = Math.sin(theta);

        // Build the matrix entries element by element.
        this.m11 = u2 + (v2 + w2) * cosT;
        this.m12 = u*v * oneMinusCosT - w*sinT;
        this.m13 = u*w * oneMinusCosT + v*sinT;
        this.m14 = (a*(v2 + w2) - u*(b*v + c*w))*oneMinusCosT
                + (b*w - c*v)*sinT;

        this.m21 = u*v * oneMinusCosT + w*sinT;
        this.m22 = v2 + (u2 + w2) * cosT;
        this.m23 = v*w * oneMinusCosT - u*sinT;
        this.m24 = (b*(u2 + w2) - v*(a*u + c*w))*oneMinusCosT
                + (c*u - a*w)*sinT;

        this.m31 = u*w * oneMinusCosT - v*sinT;
        this.m32 = v*w * oneMinusCosT + u*sinT;
        this.m33 = w2 + (u2 + v2) * cosT;
        this.m34 = (c*(u2 + v2) - w*(a*u + b*v))*oneMinusCosT
                + (a*v - b*u)*sinT;

        if(RotationMatrix.LOG) logMatrix();
    }


    // Methods-----------------------------------------------------------

    /**
     * Multiply this {@link RotationMatrix} times the point (x, y, z, 1),
     * representing a point P(x, y, z) in homogeneous coordinates.  The final
     * coordinate, 1, is assumed.
     *
     * @param x The point's x-coordinate.
     * @param y The point's y-coordinate.
     * @param z The point's z-coordinate.
     * @return The product, in a vector <#, #, #>, representing the
     * rotated point.
     */
    timesXYZ(x, y, z) {
        let p = [];
        p[0] = this.m11*x + this.m12*y + this.m13*z + this.m14;
        p[1] = this.m21*x + this.m22*y + this.m23*z + this.m24;
        p[2] = this.m31*x + this.m32*y + this.m33*z + this.m34;

        return p;
    }


    // /**
    //  * Multiply this matrix times the given coordinates (x, y, z, 1),
    //  * representing a point P(x, y, z).  This delegates to
    //  * {@link #timesXYZ(double, double, double)}, calling
    //  * <pre>
    //  *     timesXYZ(point[0], point[1], point[2]);
    //  * </pre>
    //  * thus it works with points given in homogeneous coordinates.
    //  *
    //  * @param point The point as double[] {x, y, z}.
    //  * @return The product, in a vector <#, #, #>, representing the
    //  * rotated point.
    //  */
    // timesXYZ(point) {
    //     return this.timesXYZ(point[0], point[1], point[2]);
    // }


    /**
     * <p>
     * Compute the rotated point from the formula given in the paper, as opposed
     * to multiplying this matrix by the given point.  Theoretically this should
     * give the same answer as {@link #timesXYZ(double[])}.  For repeated
     * calculations this will be slower than using {@link #timesXYZ(double[])}
     * because, in effect, it repeats the calculations done in the constructor.
     * </p>
     * <p>This method is static partly to emphasize that it does not
     * mutate an instance of {@link RotationMatrix}, even though it uses
     * the same parameter names as the the constructor.</p>
     *
     * @param a x-coordinate of a point on the line of rotation.
     * @param b y-coordinate of a point on the line of rotation.
     * @param c z-coordinate of a point on the line of rotation.
     * @param u x-coordinate of the line's direction vector.  This direction
     *          vector will be normalized.
     * @param v y-coordinate of the line's direction vector.
     * @param w z-coordinate of the line's direction vector.
     * @param x The point's x-coordinate.
     * @param y The point's y-coordinate.
     * @param z The point's z-coordinate.
     * @param theta The angle of rotation, in radians.
     * @return The product, in a vector <#, #, #>, representing the
     * rotated point.
     */
    static rotPointFromFormula(a, b, c, u, v, w, x, y, z, theta) {
        // We normalize the direction vector.
        let l;
        if ( (l = RotationMatrix.longEnough(u, v, w)) < 0) {
            console.log("RotationMatrix direction vector too short");
            return null;             // Don't bother.
        }
        // Normalize the direction vector.
        u = u/l;  // Note that is not "this.u".
        v = v/l;
        w = w/l;
        // Set some intermediate values.
        let u2 = u*u;
        let v2 = v*v;
        let w2 = w*w;
        let cosT = Math.cos(theta);
        let oneMinusCosT = 1 - cosT;
        let sinT = Math.sin(theta);

        // Use the formula in the paper.
        let p = [];
        p[0] = (a*(v2 + w2) - u*(b*v + c*w - u*x - v*y - w*z)) * oneMinusCosT
                + x*cosT
                + (-c*v + b*w - w*y + v*z)*sinT;

        p[1] = (b*(u2 + w2) - v*(a*u + c*w - u*x - v*y - w*z)) * oneMinusCosT
                + y*cosT
                + (c*u - a*w + w*x - u*z)*sinT;

        p[2] = (c*(u2 + v2) - w*(a*u + b*v - u*x - v*y - w*z)) * oneMinusCosT
                + z*cosT
                + (-b*u + a*v - v*x + u*y)*sinT;

        return p;
    }


    /**
     * <p>Compute the rotated point from the formula given in the paper,
     * as opposed to multiplying this matrix by the given point.</p>
     * 
     * <p>This delegates to {@link #rotPointFromFormula(double, double, double,
     * double, double, double, double, double, double, double)}. </p>.
     *
     * @param rInfo The information for the rotation as an array
     * [a,b,c,u,v,w,x,y,z,theta].
     * @return The product, in a vector <#, #, #>, representing the
     * rotated point.
     */
    static rotPointFromFormula(rInfo) {
        return RotationMatrix.rotPointFromFormula(rInfo[0], rInfo[1], rInfo[2], 
                                   rInfo[3], rInfo[4], rInfo[5], 
                                   rInfo[6], rInfo[7], rInfo[8], rInfo[9]);
    }


    /**
     * Check whether a vector's length is less than {@link #TOLERANCE}.
     *
     * @param u The vector's x-coordinate.
     * @param v The vector's y-coordinate.
     * @param w The vector's z-coordinate.
     * @return length = Math.sqrt(u^2 + v^2 + w^2) if it is greater than
     * {@link #TOLERANCE}, or -1 if not.
     */
    static longEnough(u, v, w) {
        let l = Math.sqrt(u*u + v*v + w*w);
        if (l > RotationMatrix.TOLERANCE) {
            return l;
        } else {
            return -1;
        }
    }


    /**
     * Get the matrix.
     * @return The matrix as a 4x4 double[][].
     */
    getMatrix() {
        if (matrix==null) {
            matrix = [
                this.m11, this.m12, this.m13, this.m14,
                this.m21, this.m22, this.m23, this.m24,
                this.m31, this.m32, this.m33, this.m34,
                0,   0,   0,   1
            ]
        }
        return matrix;
    }


    // Inner classes-----------------------------------------------------
}

export default RotationMatrix;