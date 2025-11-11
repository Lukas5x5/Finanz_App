// Supabase Edge Function: Thumbnails
// Generates thumbnails for uploaded invoice images
// Triggered after file upload to Storage

import { serve } from "https://deno.land/std@0.168.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.38.4";

serve(async (req) => {
  try {
    const supabaseUrl = Deno.env.get("SUPABASE_URL")!;
    const supabaseServiceKey = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY")!;
    const supabase = createClient(supabaseUrl, supabaseServiceKey);

    // Parse request body
    const { objectPath, contentType } = await req.json();

    // Only process images
    if (!contentType?.startsWith("image/")) {
      return new Response(
        JSON.stringify({
          success: false,
          message: "Not an image file",
        }),
        { headers: { "Content-Type": "application/json" }, status: 400 }
      );
    }

    // Download original image from storage
    const { data: originalFile, error: downloadError } = await supabase.storage
      .from("invoices")
      .download(objectPath);

    if (downloadError || !originalFile) {
      console.error("Error downloading file:", downloadError);
      return new Response(
        JSON.stringify({
          success: false,
          error: "Failed to download original file",
        }),
        { headers: { "Content-Type": "application/json" }, status: 500 }
      );
    }

    // Convert to ArrayBuffer for image processing
    const arrayBuffer = await originalFile.arrayBuffer();
    const uint8Array = new Uint8Array(arrayBuffer);

    // In a real implementation, you would:
    // 1. Use an image processing library (e.g., sharp, imageMagick)
    // 2. Resize the image to thumbnail dimensions (e.g., 200x200)
    // 3. Compress the image
    // 4. Upload the thumbnail to storage

    // For this example, we'll just create a marker file
    // In production, integrate with Deno image processing or external service

    const thumbnailPath = `thumbnails/${objectPath}`;

    // Placeholder: In production, upload actual resized image
    const { error: uploadError } = await supabase.storage
      .from("invoices")
      .upload(thumbnailPath, uint8Array, {
        contentType: contentType,
        upsert: true,
      });

    if (uploadError) {
      console.error("Error uploading thumbnail:", uploadError);
      return new Response(
        JSON.stringify({
          success: false,
          error: "Failed to upload thumbnail",
        }),
        { headers: { "Content-Type": "application/json" }, status: 500 }
      );
    }

    // Update attachment record with thumbnail path (optional)
    // await supabase.from('attachments').update({
    //   thumbnail_path: thumbnailPath
    // }).eq('object_path', objectPath);

    return new Response(
      JSON.stringify({
        success: true,
        originalPath: objectPath,
        thumbnailPath: thumbnailPath,
        message: "Thumbnail created successfully",
      }),
      {
        headers: { "Content-Type": "application/json" },
        status: 200,
      }
    );
  } catch (error) {
    console.error("Error in thumbnails function:", error);
    return new Response(
      JSON.stringify({
        success: false,
        error: error.message,
      }),
      {
        headers: { "Content-Type": "application/json" },
        status: 500,
      }
    );
  }
});
