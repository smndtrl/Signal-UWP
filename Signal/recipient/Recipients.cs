

using Signal.Models;
/** 
* Copyright (C) 2015 smndtrl
* 
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure.recipient
{
    public class Recipients// implements Iterable<Recipient>, RecipientModifiedListener 

    {

        /*
            private final Set<RecipientsModifiedListener> listeners = Collections.newSetFromMap(new WeakHashMap<RecipientsModifiedListener, Boolean>());*/
            public List<Recipient> RecipientList { get; private set; }

        private Uri ringtone = null;
        private long mutedUntil = 0;
        private bool blocked = false;
        //private VibrateState vibrate = VibrateState.DEFAULT;

        internal Recipients()
            : this(new List<Recipient>(), /*(RecipientsPreferences)*/null)
        {
        }

        public Recipients(Recipient recipient) // TODO ???
            :this(new List<Recipient>() { recipient }, null)
        {
        }

        public Recipients(List<Recipient> recipients, /*RecipientsPreferences?*/object preferences)
        {
            this.RecipientList = recipients;

        }
        /*
        public synchronized @Nullable Uri getRingtone()
        {
            return ringtone;
        }

        public void setRingtone(Uri ringtone)
        {
            lock (this)
            {
                this.ringtone = ringtone;
            }

            notifyListeners();
        }
        
        public synchronized boolean isMuted()
        {
            return System.currentTimeMillis() <= mutedUntil;
        }

        public void setMuted(long mutedUntil)
        {
            synchronized(this) {
                this.mutedUntil = mutedUntil;
            }

            notifyListeners();
        }

        public synchronized boolean isBlocked()
        {
            return blocked;
        }

        public void setBlocked(boolean blocked)
        {
            synchronized(this) {
                this.blocked = blocked;
            }

            notifyListeners();
        }

        public synchronized VibrateState getVibrate()
        {
            return vibrate;
        }

        public void setVibrate(VibrateState vibrate)
        {
            synchronized(this) {
                this.vibrate = vibrate;
            }

            notifyListeners();
        }

        public Drawable getContactPhoto(Context context)
        {
            if (recipients.size() == 1) return recipients.get(0).getContactPhoto();
            else return ContactPhotoFactory.getDefaultGroupPhoto(context);
        }

        public synchronized void addListener(RecipientsModifiedListener listener)
        {
            if (listeners.isEmpty())
            {
                for (Recipient recipient : recipients)
                {
                    recipient.addListener(this);
                }
            }

            synchronized(this) {
                listeners.add(listener);
            }
        }

        public synchronized void removeListener(RecipientsModifiedListener listener)
        {
            listeners.remove(listener);

            if (listeners.isEmpty())
            {
                for (Recipient recipient : recipients)
                {
                    recipient.removeListener(this);
                }
            }
        }

        public boolean isEmailRecipient()
        {
            for (Recipient recipient : recipients)
            {
                if (NumberUtil.isValidEmail(recipient.getNumber()))
                    return true;
            }

            return false;
        }

        public boolean isGroupRecipient()
        {
            return isSingleRecipient() && GroupUtil.isEncodedGroup(recipients.get(0).getNumber());
        }*/
        public bool IsGroupRecipient
        {
            get
            {
                return IsSingleRecipient && false; // TODO: group enable
            }
        }


        /*public bool isEmpty()
        {
            return this.recipients.Count == 0;
        }*/
        public bool IsEmpty
        {
            get
            {
                return this.RecipientList.Count == 0;
            }
        }

        /*public bool isSingleRecipient()
        {
            return this.recipients.Count == 1;
        }*/

        public bool IsSingleRecipient
        {
            get
            {
                return this.RecipientList.Count == 1;
            }
        }

        public Recipient getPrimaryRecipient() // TODO: null
        {
            if (!IsEmpty)
                return this.RecipientList[0];
            else
                return null;
        }

        public Recipient PrimaryRecipient
        {
            get
            {
                if (!IsEmpty)
                    return this.RecipientList[0];
                else
                    return null;
            }
        }

        public List<Recipient> getRecipientsList()
        {
            return this.RecipientList;
        }
        
        public long[] getIds()
        {
            long[] ids = new long[RecipientList.Count];
            for (int i = 0; i < RecipientList.Count; i++)
            {
                ids[i] = RecipientList[i].getRecipientId();
            }
            return ids;
        }
        /*
        public String getSortedIdsString()
        {
            Set<Long> recipientSet = new HashSet<>();

            for (Recipient recipient : this.recipients)
            {
                recipientSet.add(recipient.getRecipientId());
            }

            long[] recipientArray = new long[recipientSet.size()];
            int i = 0;

            for (Long recipientId : recipientSet)
            {
                recipientArray[i++] = recipientId;
            }

            Arrays.sort(recipientArray);

            return Util.join(recipientArray, " ");
        }

        public String[] toNumberStringArray(boolean scrub)
        {
            String[] recipientsArray = new String[recipients.size()];
            Iterator<Recipient> iterator = recipients.iterator();
            int i = 0;

            while (iterator.hasNext())
            {
                String number = iterator.next().getNumber();

                if (scrub && number != null &&
                    !Patterns.EMAIL_ADDRESS.matcher(number).matches() &&
                    !GroupUtil.isEncodedGroup(number))
                {
                    number = number.replaceAll("[^0-9+]", "");
                }

                recipientsArray[i++] = number;
            }

            return recipientsArray;
        }
        */

        public string ShortString
        {
            get
            {
                String fromString = "";

                for (int i = 0; i < RecipientList.Count; i++)
                {
                    fromString += RecipientList[i].ShortString;

                    if (i != RecipientList.Count - 1)
                        fromString += ", ";
                }

                return fromString;
            }
        }



        /*
        @Override
  public Iterator<Recipient> iterator()
        {
            return recipients.iterator();
        }

        @Override
  public void onModified(Recipient recipient)
        {
            notifyListeners();
        }

        private void notifyListeners()
        {
            Set<RecipientsModifiedListener> localListeners;

            synchronized(this) {
                localListeners = new HashSet<>(listeners);
            }

            for (RecipientsModifiedListener listener : localListeners)
            {
                listener.onModified(this);
            }
        }


        public interface RecipientsModifiedListener
        {
            public void onModified(Recipients recipient);
        }*/

    }
}
