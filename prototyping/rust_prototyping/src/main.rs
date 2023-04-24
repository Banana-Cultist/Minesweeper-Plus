use std::rc::Rc;

use crate::{polygon::Polygonization, float2::Float2};

pub mod float2;
pub mod polygon;
pub mod errors;

fn main() {
    let mut polygonization = Polygonization::as_squares(1).unwrap();
    println!("{:#?}\n\n", polygonization);
    // let target: Float2 = Float2 { x: 0.1, y: 0.1 };
    polygonization.vertexes[0] = Rc::new(Float2 { x: 0.1, y: 0.1 });
    println!("{:#?}", polygonization);
    assert!(**polygonization.polygons.get(0).unwrap().vertexes.get(0).unwrap() == Float2 { x: 0.1, y: 0.1 });
}
