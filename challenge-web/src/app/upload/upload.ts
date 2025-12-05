import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom, timeout, catchError } from 'rxjs';
import { of } from 'rxjs';

interface ImportResponse {
  url: string;
  filename: string;
}

interface ProcessResponse {
  totalLines: number;
  SuccessCount: number;
  FailedCount: number;
}

@Component({
  selector: 'app-upload',
  imports: [CommonModule],
  templateUrl: './upload.html',
  styleUrl: './upload.css',
})
export class Upload implements OnInit {
  presignedUrl: string | null = null;
  filename: string | null = null;
  isLoading: boolean = false;
  isUploading: boolean = false;
  errorMessage: string | null = null;
  selectedFile: File | null = null;
  showDialog: boolean = false;
  processResult: ProcessResponse | null = null;

  constructor(
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    console.log('Upload component initialized');
    this.fetchPresignedUrl();
  }

  async fetchPresignedUrl(): Promise<void> {
    this.isLoading = true;
    this.errorMessage = null;
    
    try {
      console.log('Fetching presigned URL from http://localhost:8080/api/v1/import');
      const response = await firstValueFrom(
        this.http.get<any>('http://localhost:8080/api/v1/import').pipe(
          timeout(10000),
          catchError((error) => {
            console.error('Request error:', error);
            throw error;
          })
        )
      );
      
      console.log('Response received:', response);
      
      if (response) {
        if (response.url && response.filename) {
          this.presignedUrl = response.url;
          this.filename = response.filename;
          console.log('Presigned URL and filename extracted successfully');
        } 
        else if (typeof response === 'object') {
          const url = response.url || response.presignedUrl || response.uploadUrl;
          const filename = response.filename || response.fileName || response.name;
          
          if (url && filename) {
            this.presignedUrl = url;
            this.filename = filename;
            console.log('Presigned URL and filename extracted from alternative properties');
          } else {
            throw new Error(`Invalid response format. Received: ${JSON.stringify(response)}`);
          }
        } else {
          throw new Error(`Unexpected response type: ${typeof response}. Received: ${JSON.stringify(response)}`);
        }
      } else {
        throw new Error('Empty response received');
      }
    } catch (error: any) {
      console.error('Error fetching presigned URL:', error);
      
      let errorMsg = 'Failed to fetch presigned URL';
      if (error?.error) {
        errorMsg = error.error.message || JSON.stringify(error.error);
      } else if (error?.message) {
        errorMsg = error.message;
      } else if (typeof error === 'string') {
        errorMsg = error;
      }
      
      if (error?.name === 'TimeoutError' || error?.error?.name === 'TimeoutError') {
        errorMsg = 'Request timed out. The server may be unreachable or there may be a CORS issue. Please check the browser console and Network tab.';
      } else if (error?.status === 0 || error?.name === 'HttpErrorResponse') {
        errorMsg = 'Network error: Could not connect to server. This may be a CORS issue. Please check if the backend is running on http://localhost:8080 and allows CORS from http://localhost:4200';
      } else if (error?.status) {
        errorMsg = `Server error (${error.status}): ${errorMsg}`;
      }
      
      this.errorMessage = errorMsg;
    } finally {
      this.isLoading = false;
      console.log('Loading completed, isLoading set to false');
      console.log('Current state:', {
        isLoading: this.isLoading,
        errorMessage: this.errorMessage,
        presignedUrl: this.presignedUrl,
        filename: this.filename
      });
      this.cdr.detectChanges();
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.errorMessage = null;
    }
  }

  async uploadFile(): Promise<void> {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file first';
      return;
    }

    if (!this.presignedUrl) {
      this.errorMessage = 'Presigned URL not available. Please try refreshing the page.';
      return;
    }

    if (!this.filename) {
      this.errorMessage = 'Filename not available. Please try refreshing the page.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = null;
    this.showDialog = false;
    this.processResult = null;

    try {
      const headers = new HttpHeaders({
        'Content-Type': this.selectedFile.type || 'application/octet-stream',
      });

      const fileContent = await this.readFileAsArrayBuffer(this.selectedFile);
      
      if (this.presignedUrl) {
        try {
          const url = new URL(this.presignedUrl);
          const credentialParam = url.searchParams.get('X-Amz-Credential');
          if (credentialParam) {
            const accessKey = credentialParam.split('/')[0];
            console.log('Access Key in presigned URL:', accessKey);
            if (accessKey === 'localhost_admin') {
              console.warn('WARNING: The presigned URL uses "localhost_admin" as the access key. MinIO requires the actual access key (e.g., "minio_admin").');
            }
          }
        } catch (e) {
          console.warn('Could not parse presigned URL:', e);
        }
      }
      
      console.log('Uploading to presigned URL:', this.presignedUrl);
      
      await firstValueFrom(
        this.http.put(this.presignedUrl!, fileContent, { headers }        )
      );

      const processResponse = await firstValueFrom(
        this.http.post<ProcessResponse>(
          'http://localhost:8080/api/v1/import',
          { filename: this.filename },
          { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) }
        )
      );

      this.processResult = processResponse;
      this.showDialog = true;
      
      this.selectedFile = null;
      const fileInput = document.getElementById('file-input') as HTMLInputElement;
      if (fileInput) {
        fileInput.value = '';
      }
    } catch (error: any) {
      console.error('Error uploading file:', error);
      console.error('Error details:', JSON.stringify(error, null, 2));
      
      let errorMsg = 'Failed to upload file';
      
      if (error?.status === 403) {
        const errorBody = error?.error || '';
        let errorText = typeof errorBody === 'string' ? errorBody : JSON.stringify(errorBody);
        
        if (typeof errorBody === 'string' && errorBody.includes('<Error>')) {
          const codeMatch = errorBody.match(/<Code>(.*?)<\/Code>/);
          const messageMatch = errorBody.match(/<Message>(.*?)<\/Message>/);
          
          if (codeMatch) {
            const errorCode = codeMatch[1];
            const errorMessage = messageMatch ? messageMatch[1] : '';
            
            console.log('MinIO Error Code:', errorCode);
            console.log('MinIO Error Message:', errorMessage);
            
            if (errorCode === 'SignatureDoesNotMatch') {
              errorMsg = `Signature mismatch: ${errorMessage || 'The presigned URL signature was calculated for a different hostname. When the backend generates the presigned URL, it must calculate the signature using "localhost:9000" as the hostname from the start (not "minio:9000"). The signature includes the Host header, so if the URL was originally signed for "minio:9000" but the browser sends it to "localhost:9000", the signature will be invalid. The backend needs to generate the presigned URL with hostname="localhost:9000" and calculate the signature accordingly.'}`;
            } else if (errorCode === 'InvalidAccessKeyId') {
              errorMsg = `Invalid Access Key: ${errorMessage || 'The Access Key ID in the presigned URL does not exist in MinIO. The backend needs to generate the presigned URL with the actual MinIO access key (e.g., "minio_admin") while keeping the hostname as "localhost:9000" for browser access.'}`;
            } else if (errorCode === 'AccessDenied') {
              errorMsg = `Access denied: ${errorMessage || 'The presigned URL does not have permission to upload to this location.'}`;
            } else {
              errorMsg = `MinIO Error (${errorCode}): ${errorMessage || 'Unknown error'}`;
            }
          } else {
            errorText = errorBody;
          }
        }
        
        if (errorMsg === 'Failed to upload file') {
          if (errorText.includes('SignatureDoesNotMatch')) {
            errorMsg = 'Signature mismatch: The presigned URL signature is invalid. Please check that the backend is generating the URL correctly.';
          } else if (errorText.includes('AccessDenied') || errorText.includes('Access Denied')) {
            errorMsg = 'Access denied: The presigned URL does not have permission to upload to this location.';
          } else {
            errorMsg = `Upload forbidden (403): ${error?.message || 'Access denied. Check browser console and Network tab for details.'}`;
          }
        }
      }
      else if (error?.status === 0 || (error?.name === 'HttpErrorResponse' && !error?.status)) {
        errorMsg = 'Network error: Could not connect to the upload server. This might be a CORS issue or the server is not accessible.';
      } else if (error?.status) {
        errorMsg = `Upload failed with status ${error.status}: ${error?.message || 'Unknown error'}`;
      } else if (error?.message) {
        errorMsg = error.message;
      }
      
      this.errorMessage = errorMsg;
    } finally {
      this.isUploading = false;
      this.cdr.detectChanges();
    }
  }

  private readFileAsArrayBuffer(file: File): Promise<ArrayBuffer> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as ArrayBuffer);
      reader.onerror = reject;
      reader.readAsArrayBuffer(file);
    });
  }


  closeDialog(): void {
    this.showDialog = false;
    this.processResult = null;
  }

  onRetry(): void {
    this.fetchPresignedUrl();
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }
}

