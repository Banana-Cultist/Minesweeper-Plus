use std::{
    fmt,
    ops::{Add, Div, Mul, Neg, Sub},
};

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct Float2 {
    pub x: f32,
    pub y: f32,
}

impl Float2 {
    // pub fn random(min: f64, max: f64) -> Float2 {
    //     let mut rng = rand::thread_rng();
    //     Float2::new(
    //         rng.gen_range(min..max),
    //         rng.gen_range(min..max),
    //         rng.gen_range(min..max),
    //     )
    // }

    // pub fn distance(&self, other: &Float2) -> f64 {
    //     let dx = self.x - other.x();
    //     let dy = self.y - other.y();
    //     (dx * dx + dy * dy).sqrt()
    // }

    pub fn length2(&self) -> f32 {
        self.x * self.x + self.y * self.y
    }

    pub fn length(&self) -> f32 {
        (self.x * self.x + self.y * self.y).sqrt()
    }

    // pub fn unit_vector(&self) -> Float2 {
    //     let length = self.length();
    //     Float2::new(self.x / length, self.y / length, self.z / length)
    // }

    // pub fn dot(&self, other: &Float2) -> f64 {
    //     self.x * other.x + self.y * other.y + self.z * other.z
    // }

    // pub fn cross(&self, other: &Float2) -> Float2 {
    //     Float2::new(
    //         self.y * other.z - self.z * other.y,
    //         self.z * other.x - self.x * other.z,
    //         self.x * other.y - self.y * other.x,
    //     )
    // }

    // pub fn near_zero(&self) -> bool {
    //     self.real.abs() < f64::EPSILON && self.imag.abs() < f64::EPSILON
    // }
}

impl Add for Float2 {
    type Output = Float2;

    fn add(self, other: Float2) -> Float2 {
        Float2 {
            x: self.x + other.x,
            y: self.y + other.y,
        }
    }
}

impl Sub for Float2 {
    type Output = Float2;

    fn sub(self, other: Float2) -> Float2 {
        Float2 {
            x: self.x - other.x,
            y: self.y - other.y,
        }
    }
}

impl Neg for Float2 {
    type Output = Float2;

    fn neg(self) -> Float2 {
        Float2 {
            x: -self.x,
            y: -self.y,
        }
    }
}

impl Mul<f32> for Float2 {
    type Output = Float2;

    fn mul(self, other: f32) -> Float2 {
        Float2 {
            x: self.x * other,
            y: self.y * other,
        }
    }
}

impl Div<f32> for Float2 {
    type Output = Float2;

    fn div(self, other: f32) -> Float2 {
        Float2 {
            x: self.x / other,
            y: self.y / other,
        }
    }
}

impl fmt::Display for Float2 {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "Float2({}, {})", self.x, self.y)
    }
}
